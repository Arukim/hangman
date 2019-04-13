const requestGameInfo = 'REQUEST_GAME_INFO';
const responseGameInfo = 'RESPONSE_GAME_INFO';
const madeTurn = "MADE_TURN";


export const signalRGameStarted = 'GAME_STARTED';
export const signalRGuess = 'GAME_GUESS';


export const Status = {
    Loading: 'LOADING',
    NotFound: 'NOT_FOUND',
    Init: 'INIT',
    InProgress: 'IN_PROGRESS',
    Won: 'WON'
};

const initialState = {
    status: Status.Loading,
    turnsLeft: 7
};

export const actionCreators = {
    requestGameInfo: id => async (dispatch, getState) => {
        dispatch({ type: requestGameInfo, id });

        const url = `api/hangman/game/${id}`;
        const response = await fetch(url);
        const gameInfo = await response.json();

        dispatch({ type: responseGameInfo, id, gameInfo });
    },
    onGuessClick: (id, guess) => async (dispatch, getState) => {
        dispatch({ type: signalRGuess, id, guess });

        dispatch({ type: madeTurn });
    }
}


export const reducer = (state, action) => {
    state = state || initialState;


    if (action.type === requestGameInfo) {
        return {
            ...state,
            id: action.id,
            status: Status.Loading
        };
    }

    if (action.type === responseGameInfo) {
        if (action.gameInfo.hasWon) {
            state.status = Status.Won;
        }

        return {
            ...state,
            id: action.id,
            gameInfo: action.gameInfo,
            status: state.status === Status.Loading ? Status.Init : state.status
        };
    }

    if (action.type === signalRGameStarted) {
        return {
            ...state,
            ...action,
            status: Status.InProgress
        };
    }

    if (action.type === madeTurn) {
        return {
            ...state,
            totalTurns: state.turnsLeft - 1
        };
    }


    return state;
}