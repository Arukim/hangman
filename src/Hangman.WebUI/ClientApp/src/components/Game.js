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
        return (
            <div>
                <h1>Game {this.props.id}</h1>
                {this.props.status === Status.Loading ? renderLoading(this.props) : renderGame(this.props)}
            </div>
        );
    }
}

function renderLoading(props) {
    return (
        <h2> Loading data </h2>
    );
}

function renderGame(props) {
    return (
        <h2> Waiting for the game to start </h2>
    );
}

export default connect(
    state => state.game,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Game);
