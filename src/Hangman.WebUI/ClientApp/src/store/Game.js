const requestGameInfo = 'REQUEST_GAME_INFO';
const madeTurn = "MADE_TURN";
const reset = "GAME_RESET";

export const gameState = 'QUERY_STATE';
export const signalRGuess = 'COMMAND_GUESS';
export const signalRSubscribe = 'COMMAND_SUBSCRIBE';
export const signalRUnsubscribe = 'COMMAND_UNSUBSCRIBE';

const initialState = {
    isLoading: true,
    turnsLeft: -1
};

export const actionCreators = {
    requestGameInfo: id => async (dispatch, getState) => {
        dispatch({ type: requestGameInfo, id });

        const url = `api/hangman/game/${id}`;
        const response = await fetch(url);
        const gameInfo = await response.json();

        dispatch({ type: gameState, ...gameInfo });
        dispatch({ type: signalRSubscribe, id: gameInfo.correlationId });
    },
    onGuessClick: (id, guess) => async (dispatch, getState) => {
        dispatch({ type: signalRGuess, id, guess });

        dispatch({ type: madeTurn });
    },
    exit: (id) => async (dispatch, getState) => {
        dispatch({ type: reset });
        dispatch({ type: signalRUnsubscribe, id: id });
    }
}


export const reducer = (state, action) => {
    state = state || initialState;


    if (action.type === requestGameInfo) {
        return {
            ...state,
            id: action.id,
            isLoading: true
        };
    }

    if (action.type === gameState) {
        return {
            ...state,
            ...action,
            isLoading: false
        };
    }

    if (action.type === madeTurn) {
        return {
            ...state
        };
    }

    if (action.type === reset) {
        return initialState;
    }

    return state;
}