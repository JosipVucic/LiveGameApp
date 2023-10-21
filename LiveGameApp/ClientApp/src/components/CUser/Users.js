import React, { Component } from 'react';
import { UserCard } from './UserCard';
import DataProvider from '../admin/DataProvider'
import authService from '../api-authorization/AuthorizeService';

export class Users extends Component {
    static displayName = Users.name;

    constructor(props) {
        super(props);
        this.state = {
            items: [],
            total: 0,
            pagination: {
                page: 1,
                perPage: 12,
                hasPrev: false,
                hasNext: false
            },
            filter: {
                FriendsOnly: false,
                q: "",
            },
            friends: [],
            isAuthenticated: false,
            userName: null,
            userId: null,
        };
    }

    populateState = async () => {
        const [isAuthenticated, user] = await Promise.all([authService.isAuthenticated(), authService.getUser()])
        this.setState({
            ...this.state,
            isAuthenticated,
            userName: user && user.name,
            userId: user && Number(user.sub),
        });
        this.fetchItems();
    }

    fetchItems() {
        const params = {
            pagination: this.state.pagination,
            sort: {
                field: "id",
                order: "ASC"
            },
            filter: this.state.filter,
        };
        let pagination = this.state.pagination;
        let hasPrev = pagination.page > 1;
        let startIndex = (pagination.page) * pagination.perPage;
        DataProvider.getList("CAppUsers", params).then(object => { this.setState({ ...this.state, pagination: { ...this.state.pagination, hasPrev: hasPrev, hasNext: startIndex < object.total }, items: object.data, total: object.total }); });

        const friendParams = {
            pagination: this.state.pagination,
            sort: {
                field: "id",
                order: "ASC"
            },
            filter: {
                q: undefined,
                FriendsOnly: true,
            },
        };

        DataProvider.getList("CAppUsers", friendParams).then(object => { this.setState({ ...this.state, friends: object.data }); });
        }

    handlePageChange = (event) => {
        this.setState({ ...this.state, pagination: { ...this.state.pagination, page: event.target.value } }, () => this.fetchItems());
    }

    handlePageInc = (event) => {
        event.preventDefault();
        let page = this.state.pagination.page + 1;
        this.setState({ ...this.state, pagination: { ...this.state.pagination, page: page } }, () => this.fetchItems());
    }

    handlePageDec = (event) => {
        event.preventDefault();
        let page = Math.max(this.state.pagination.page - 1, 1);
        this.setState({ ...this.state, pagination: { ...this.state.pagination, page: page } }, () => this.fetchItems());
    }

    componentDidMount() {
        this._subscription = authService.subscribe(() => this.populateState());
        this.populateState();
    }

    componentWillUnmount() {
        authService.unsubscribe(this._subscription);
    }

    handleFilter = (event) => {
        event.preventDefault();

        const filter = { ...this.state.filter};

        const FriendsOnly = !filter.FriendsOnly;
        filter.FriendsOnly = FriendsOnly;

        this.setState({
            ...this.state, filter: filter, pagination: {
                page: 1,
                perPage: 12,
                hasPrev: false,
                hasNext: false
            } }, () => this.fetchItems());

    }

    handleChange = (event) => {
        const target = event.target;
        let value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;

        if (value == "") value = undefined;

        const filter = { ...this.state.filter, [name]: value };

        this.setState({
            ...this.state,
            filter: filter,
        }, () => this.fetchItems());
    }

    render() {

        return (

            <div className="container-fluid">
                <div className="text-center m-5">
                    <h1 className="display-4">Users</h1>
                </div>

                <nav aria-label="Page navigation">
                    <form>

                        <ul className="pagination justify-content-center">
                            <li className="page-item">
                                <button onClick={this.handleFilter} className="page-link bg-dark text-light">{this.state.filter.FriendsOnly ? "Friends" : "All"}</button>
                            </li>
                            <li className="page-item">
                                <input type="text" className="page-link text-center" name="q" value={this.state.filter.q} placeholder="Search Username" onChange={this.handleChange} />
                            </li>
                            <li className="page-item">
                                <button className="page-link" disabled={!this.state.pagination.hasPrev} onClick={this.handlePageDec}>Previous</button>
                            </li>
                            <li className="page-item">
                                <input type="number" className="page-link text-center" value={this.state.pagination.page} onChange={this.handlePageChange} />
                            </li>
                            <li className="page-item">
                                <button className="page-link" disabled={!this.state.pagination.hasNext} onClick={this.handlePageInc}>Next</button>
                            </li>

                        </ul>
                    </form>
                </nav>

                {this.filterMenu}

                <div className="container-fluid">
                    <div className="d-flex flex-wrap align-items-stretch">
                        {this.state.items.map((value, index) => {
                            if (value.id == this.state.userId) return null;
                            return <UserCard item={value} senderId={this.state.userId} senderName={this.state.userName} isFriend={value.IsFriend} refresh={this.populateState} />
                        })}




                    </div>
                </div>

            </div>
        );
    }
}
