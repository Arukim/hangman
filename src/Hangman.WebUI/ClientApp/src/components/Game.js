import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { actionCreators, Status } from '../store/Game';


class Game extends Component {
    componentDidMount() {
        this.init();
    }

    init() {
        this.props.requestGameInfo(this.props.match.params.id);
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
                game = renderGameInProgress(this.props);
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

function renderGameInProgress(props) {
    return (
        <div>
            <h2> Game is in progress </h2>
            <h3> You have {props.TurnsLeft} turns left </h3>
        </div>
    );
}

export default connect(
    state => state.game,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Game);
