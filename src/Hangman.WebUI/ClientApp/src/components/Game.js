import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { actionCreators } from '../store/Game';


class Game extends Component {

    constructor() {
        super();
        this.state = { guess: '' };
    }

    componentDidMount() {
        this.init();
    }

    componentWillUnmount() {
        console.log("unmount");
        this.props.exit(this.props.correlationId);
    }

    init() {
        this.props.requestGameInfo(this.props.match.params.id);
    }

    onGuessStateChange = (e) => {
        this.setState({ guess: e.target.value });
    }

    onSubmitTurn = (e) => {
        e.preventDefault();
        let guess = this.state.guess;
        if (guess.length > 0 && this.props.turnsLeft > 0) {
            this.setState({ guess: "" },
                () => this.props.onGuessClick(this.props.correlationId, guess)
            );
        };
    }

    render() {
        let game;
        if (this.props.isLoading) {
            game = this.renderGameLoading();
        } else {
            game = this.renderGameInProgress();
        }

        let img = this.props.hasWon ? "won" : this.props.turnsLeft;

        return (
            <div>
                <img src={`/img/hangman/${img}.png`} alt="game state" style={{ border: '2px solid black' }} />
                {game}
            </div>
        );
    }

    renderGameInProgress() {
        return (
            <div>
                {
                    this.props.turnsLeft >= 0 ?
                        <div>
                            <h3> You have {this.props.turnsLeft} turns left </h3>

                            <h3> {this.props.guessedWord} </h3>
                            <form onSubmit={this.onSubmitTurn}>
                                <input
                                    type="text"
                                    onChange={this.onGuessStateChange}
                                    value={this.state.guess}
                                    ref={input => input && input.focus()}
                                ></input>
                                <button> Guess </button>
                            </form>
                        </div>
                        : null
                }
                <div>
                    {
                        this.props.guesses ?
                            this.props.guesses.map((x, i) =>
                                <span key={i}>{x},</span>
                            ) : null
                    }
                </div>
            </div>
        );
    }

    renderGameLoading() {
        return (
            <h2> Loading data </h2>
        );
    }
}

export default connect(
    state => state.game,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Game);
