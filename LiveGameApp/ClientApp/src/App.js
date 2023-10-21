import React, { Component } from 'react';
import './custom.css'

import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Administration } from './components/Admin'
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';
import { Games } from './components/CGame/Games';
import { MyGames } from './components/CGame/MyGames';
import { Game } from './components/CGame/Game';
import { Plans } from './components/CPlan/Plans';
import { Plan } from './components/CPlan/Plan';
import { GameCreate } from './components/CGame/GameCreate';
import { Users } from './components/CUser/Users';
import { User } from './components/CUser/User';
import { Inbox } from './components/CChat/Inbox';
import { PlanCreate } from './components/CPlan/PlanCreate';




export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Layout>
                <Route exact path='/' component={Home} />

                <AuthorizeRoute exact path='/userlist' component={Users} />
                <AuthorizeRoute exact path='/userlist/:id' component={User} />

                <Route exact path='/gamelist/:id' component={Game} />
                <Route exact path='/gamelist' component={Games} />
                <AuthorizeRoute exact path='/gamecreate' component={GameCreate} />
                <AuthorizeRoute exact path='/library' component={MyGames} />

                <AuthorizeRoute exact path='/planlist/:id' component={Plan} />
                <AuthorizeRoute exact path='/planlist' component={Plans} />
                <AuthorizeRoute exact path='/plancreate/:id' component={PlanCreate} />

                <AuthorizeRoute path='/admin' component={Administration} />
                <AuthorizeRoute path='/inbox' component={Inbox} />

                <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
            </Layout>
        );
    }
}
