import { applyMiddleware, combineReducers, compose, createStore } from 'redux';
import * as SignalR from '@aspnet/signalr';
import thunk from 'redux-thunk';
import { routerReducer, routerMiddleware } from 'react-router-redux';
import * as Counter from './Counter';
import * as WeatherForecasts from './WeatherForecasts';
import * as Home from './Home';
import * as Game from './Game';

export function configureStore(history, initialState) {
    const reducers = {
        counter: Counter.reducer,
        weatherForecasts: WeatherForecasts.reducer,
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
    .withUrl("/SignalRCounter")
    .configureLogging(SignalR.LogLevel.Information)
    .build();

export function signalRInvokeMiddleware(store) {
    return (next) => async (action) => {
        switch (action.type) {
            case "SIGNALR_INCREMENT_COUNT":
                connection.invoke('IncrementCounter');
                break;
            case "SIGNALR_DECREMENT_COUNT":
                connection.invoke('DecrementCounter');
                break;
            case Game.signalRGuess:
                connection.invoke('Guess', action.id, action.guess);
                break;
        }

        return next(action);
    }
}

export function signalRRegisterCommands(store, callback) {

    connection.on('IncrementCounter', data => {
        store.dispatch({ type: 'INCREMENT_COUNT' });
        console.log("Count has been incremented");
    });

    connection.on('DecrementCounter', data => {
        store.dispatch({ type: 'DECREMENT_COUNT' });
        console.log("Count has been decremented");
    });
    
    connection.on('GameStarted', data => {
        store.dispatch({ ...data, type: Game.signalRGameStarted });
    });

    connection.start().then(callback());

}