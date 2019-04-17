import { applyMiddleware, combineReducers, compose, createStore } from 'redux';
import * as SignalR from '@aspnet/signalr';
import thunk from 'redux-thunk';
import { routerReducer, routerMiddleware } from 'react-router-redux';
import * as Home from './Home';
import * as Game from './Game';

export function configureStore(history, initialState) {
    const reducers = {
        home: Home.reducer,
        game: Game.reducer
    };

    const middleware = [
        thunk,
        routerMiddleware(history),
        signalRInvokeMiddleware
    ];

    // In development, use the browser's Redux dev tools extension if installed
    const enhancers = [];
    const isDevelopment = process.env.NODE_ENV === 'development';
    if (isDevelopment && typeof window !== 'undefined' && window.devToolsExtension) {
        enhancers.push(window.devToolsExtension());
    }

    const rootReducer = combineReducers({
        ...reducers,
        routing: routerReducer
    });

    return createStore(
        rootReducer,
        initialState,
        compose(applyMiddleware(...middleware), ...enhancers)
    );
}


const connection = new SignalR.HubConnectionBuilder()
    .withUrl("/SignalR")
    .configureLogging(SignalR.LogLevel.Information)
    .build();

export function signalRInvokeMiddleware(store) {
    return (next) => async (action) => {
        switch (action.type) {
            case Game.signalRGuess:
                connection.invoke('Guess', action.id, action.guess);
                break;
            case Game.signalRSubscribe:
                connection.invoke('Subscribe', action.id);
                break;
            default:
                break;
        }

        return next(action);
    }
}

export function signalRRegisterCommands(store, callback) {
    
    connection.on('GameState', data => {
        store.dispatch({ ...data, type: Game.gameState });
    });

    connection.start().then(callback());

}