import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';

export class Layout extends Component {
    static displayName = Layout.name;

    render() {
        return (
            <div className="text-light h-100" >
                <NavMenu />
                <Container fluid={true} className="h-100">
                    {this.props.children}
                </Container>
            </div>
        );
    }
}
