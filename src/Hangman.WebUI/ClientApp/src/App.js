import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import Game from './components/Game';

export default () => (
    <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/game/:id' component={Game} />
    </Layout>
);
