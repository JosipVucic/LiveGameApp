import React, { Component } from 'react';

export class FilterPill extends Component {
    static displayName = FilterPill.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div class="btn btn-secondary m-1 disabled">{this.props.Name}
                <button onClick={this.props.close} value={this.props.Value} class="btn close p-0 ml-2">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        );
    }
}