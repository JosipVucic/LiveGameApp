import React, { Component } from 'react';
import { PlanCard } from '../CPlan/PlanCard';
import DataProvider from '../admin/DataProvider'
import authService from '../api-authorization/AuthorizeService';

export class Plans extends Component {
    static displayName = Plans.name;

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
            filterMode: false,
            filter: {
                q: undefined,
                Type: undefined,
                PrivacyType: undefined,
                ShowMine: false,
            }
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
        DataProvider.getList("CPlans", params)
            .then(object => {
                this.setState(
                    {
                        ...this.state,
                        pagination:
                        {
                            ...this.state.pagination,
                            hasPrev: hasPrev,
                            hasNext: startIndex < object.total
                        },
                        items: object.data,
                        total: object.total
                    });
            });
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

    filterMenu = null;

    handleFilterMenu = (event) => {
        event.preventDefault();

        const filterMode = !this.state.filterMode;
        this.setState({ ...this.state, filterMode: filterMode });
    }

    handleFilter = (event) => {
        event.preventDefault();

        this.fetchItems()
    }

    handleReset = (event) => {
        event.preventDefault();

        this.setState({
            ...this.state,
            filter:
            {
                q: undefined,
                Type: undefined,
                PrivacyType: undefined,
                ShowMine: false,
            }
        }, () => this.fetchItems());

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
            pagination: {
                page: 1,
                perPage: 12,
                hasPrev: false,
                hasNext: false,
            }
        }, () => this.fetchItems());
    }

    render() {

        if (this.state.filterMode) {
            this.filterMenu =
                <div className="d-flex m-2 align-items-center justify-content-center">
                <div className="card bg-dark col-sm-12 col-md-10 col-lg-6">
                        <div className="card-body px-3 pt-3 pb-0">
                            <form>
                                <div className="form-group">
                                    <label for="PlanName">Name</label>
                                    <input type="text" name="q" id="PlanName" value={this.state.filter.q} onChange={this.handleChange} className="form-control w-75" />
                                </div>
                                <div className="form-group">
                                    <label for="PlanType">Type</label>
                                <select name="Type" onChange={this.handleChange} className="form-control  w-50" id="PlanType">
                                        <option selected={this.state.filter.Type == undefined} value="" >Undefined</option>
                                        <option selected={this.state.filter.Type == "Game"} value="Game">Game</option>
                                        <option selected={this.state.filter.Type == "Tournament"} value="Tournament">Tournament</option>
                                    </select>
                                </div>
                                <div className="form-group">
                                    <label for="PlanPrivacyType">Privacy type</label>
                                <select name="PrivacyType" onChange={this.handleChange} className="form-control  w-50" id="PlanPrivacyType">
                                        <option selected={this.state.filter.PrivacyType == undefined} value="" >Undefined</option>
                                        <option selected={this.state.filter.PrivacyType == "Private"} value="Private">Private</option>
                                        <option selected={this.state.filter.PrivacyType == "Public"} value="Public">Public</option>
                                    </select>
                                </div>
                                <div className="form-group ml-3">

                                    <input type="checkbox" checked={this.state.filter.ShowMine} name="ShowMine" onChange={this.handleChange} className="form-check-input" id="PlanShowMine" />
                                    <label className="form-check-label" for="PlanShowMine">Show only mine</label>
                                </div>
                                <div className="form-group d-flex">
                                    <button className="form-control btn mx-1 btn-secondary" onClick={this.handleReset}>Reset</button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>;
        } else {
            this.filterMenu = null
        }

        return (

            <div className="container-fluid">
                <div className="text-center m-5">
                    <h1 className="display-4">Plans</h1>
                </div>

                <nav aria-label="Page navigation">
                    <form>

                        <ul className="pagination justify-content-center">
                            <li className="page-item">
                                <button onClick={this.handleFilterMenu} className="page-link bg-dark text-light">Filters</button>
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
                            return <PlanCard item={value} isAuthenticated={this.state.isAuthenticated} />
                        })}




                    </div>
                </div>

            </div>
        );
    }
}
