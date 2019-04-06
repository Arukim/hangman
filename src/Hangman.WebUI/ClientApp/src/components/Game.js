import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { actionCreators, Status } from '../store/Game';


class Game extends Component {

    constructor() {
        super();
        this.state = { guess: ''};
    }

    componentDidMount() {
        this.init();
    }

    init() {
        this.props.requestGameInfo(this.props.match.params.id);
    }

    onGuessStateChange = (e) => {
        this.setState({ guess: e.target.value });
    }

    onGuessClick = (e) => {
        let guess = this.state.guess;
        if (guess.length > 0 && this.props.turnsLeft > 0) {
            this.setState({ guess: "" },
                () => this.props.onGuessClick(this.props.correlationId, guess)
            );
        };
    }

    render() {
        let game;
        switch (this.props.status) {
            case Status.Init:
                game = renderGameInit(this.props);
                break;
            case Status.Loading:
                game = renderGameLoading(this.props);
                break;
            case Status.InProgress:
                game = this.renderGameInProgress();
                break;
            default:
                break;
        }
        return (
            <div>
                <h1>Game {this.props.id}</h1>
                {game}
            </div>
        );
    }

    renderGameInProgress() {
        return (
            <div>
                <h2> Game is in progress </h2>
                <h3> You have {this.props.turnsLeft} turns left </h3>
                <h3> {this.props.guessedWord} </h3>
                <input
                    type="text"
                    onChange={this.onGuessStateChange}
                    value={this.state.guess}
                ></input>
                <button onClick={this.onGuessClick}> Guess </button>
                <ul>
                    {this.props.guesses.map(x => 
                        <li>{x}</li>
                    )}
                </ul>
            </div>
        );
    }
}

function renderGameLoading(props) {
    return (
        <h2> Loading data </h2>
    );
}

function renderGameInit(props) {
    return (
        <h2> Waiting for the game to start </h2>
    );
}

export default connect(
    state => state.game,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Game);
