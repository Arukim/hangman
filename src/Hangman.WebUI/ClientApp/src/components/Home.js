import React from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { actionCreators } from '../store/Home';

const Home = props => (
    <div>
        <h1>Welcome to the Game</h1>

        <br />
        <h2> This is a demo instance of Hangman game, built with:</h2>

        <ul>
            <li><a href='https://get.asp.net/'>ASP.NET Core</a> and <a href='https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx'>C#</a> for cross-platform server-side code</li>
            <li><a href='https://www.rabbitmq.com/'>RabbitMQ</a> as a general purpose event queue with AMQP support </li>
            <li><a href='https://masstransit-project.com/'>MassTransit</a> as a messaging library and <a href='http://masstransit-project.com/MassTransit/advanced/sagas/'> MassTransit Saga </a> as a saga provider </li>
            <li><a href='https://www.mongodb.com/'> MongoDB </a> for data persistence </li>
            <li><a href='https://facebook.github.io/react/'>React</a> and <a href='https://redux.js.org/'>Redux</a> for client-side code</li>
            <li><a href='http://getbootstrap.com/'>Bootstrap</a> for layout and styling</li>
            <li><a href='https://www.docker.com/'> Docker </a> as a containerization engine </li>
        </ul>

        <h2>To get started, you can:</h2>
        <ul>
            <li><strong>Navigate to sources</strong>. Please, browse sources at <a href='https://github.com/Arukim/hangman'>github</a>.</li>
            <li><strong>Test the application</strong>. Press <strong> create game </strong> button to start a game.</li>
        </ul>
        <br/>
        <button className="btn btn-primary" onClick={props.createGame}>Start Game</button>
    </div>
);

export default connect(
    state => state.home,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Home);
