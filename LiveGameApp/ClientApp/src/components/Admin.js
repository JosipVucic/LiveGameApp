import React, { Component } from 'react';
import { Admin, Resource, ListGuesser, EditGuesser } from 'react-admin';
import dataProvider from './admin/DataProvider';

import { AppuserList, AppuserEdit, AppuserCreate } from './admin/entities/AppUser.js';
import { EnumList, EnumEdit, EnumCreate } from './admin/entities/Enum';

import { DirectmessageList, DirectmessageEdit, DirectmessageCreate } from './admin/entities/DirectMessage';
import { MessageList, MessageEdit, MessageCreate } from './admin/entities/Message';
import { FriendrequestList, FriendrequestEdit, FriendrequestCreate } from './admin/entities/FriendRequest';
import { InvitationList, InvitationEdit, InvitationCreate } from './admin/entities/Invitation';
import { ParticipationRequestList, ParticipationRequestEdit, ParticipationRequestCreate } from './admin/entities/ParticipationRequest';

import { GameList, GameEdit, GameCreate } from './admin/entities/Game';
import { PlanList, PlanEdit, PlanCreate } from './admin/entities/Plan';

import { AuthorList, AuthorCreate } from './admin/entities/Author';
import { UserPlanList, UserPlanCreate } from './admin/entities/UserPlan'
import { FriendList, FriendCreate } from './admin/entities/Friend';
import { HasRoleList, HasRoleCreate } from './admin/entities/HasRole';
import { IsGenreList, IsGenreCreate } from './admin/entities/IsGenre';
import { OwnList, OwnCreate } from './admin/entities/Owns';
import { ParticipantList, ParticipantCreate } from './admin/entities/Participant';
import { ReviewList, ReviewEdit, ReviewCreate } from './admin/entities/Reviews';


export class Administration extends Component {
    static displayName = Administration.name;

    render() {
        return (
            <Admin dataProvider={dataProvider} >
                <Resource name="AppUsers" list={AppuserList} edit={AppuserEdit} create={AppuserCreate} />
                <Resource name="Authors" list={AuthorList} create={AuthorCreate} />
                <Resource name="DirectMessages" list={DirectmessageList} edit={DirectmessageEdit} create={DirectmessageCreate} />
                <Resource name="Friends" list={FriendList} create={FriendCreate} />
                <Resource name="FriendRequests" list={FriendrequestList} edit={FriendrequestEdit} create={FriendrequestCreate} />
                <Resource name="Games" list={GameList} edit={GameEdit} create={GameCreate} />
                <Resource name="GameGenres" list={EnumList} edit={EnumEdit} create={EnumCreate} />
                <Resource name="HasRoles" list={HasRoleList} create={HasRoleCreate} />
                <Resource name="Invitations" list={InvitationList} edit={InvitationEdit} create={InvitationCreate} />
                <Resource name="InviteRequestTypes" list={EnumList} edit={EnumEdit} create={EnumCreate} />
                <Resource name="IsGenres" list={IsGenreList} create={IsGenreCreate} />
                {/*<Resource name="Messages" list={MessageList} edit={MessageEdit} create={MessageCreate} />*/}
                <Resource name="Owns" list={OwnList} create={OwnCreate} />
                {/*<Resource name="Participants" list={ParticipantList} create={ParticipantCreate} />*/}
                <Resource name="ParticipationRequests" list={ParticipationRequestList} edit={ParticipationRequestEdit} create={ParticipationRequestCreate} />
                <Resource name="Plans" list={PlanList} edit={PlanEdit} create={PlanCreate}/>
                <Resource name="PlanTypes" list={EnumList} edit={EnumEdit} create={EnumCreate} />
                <Resource name="Players" list={UserPlanList} create={UserPlanCreate} />
                <Resource name="PrivacyTypes" list={EnumList} edit={EnumEdit} create={EnumCreate} />

                <Resource name="Reviewables" list={ListGuesser} create={EnumCreate} />

                <Resource name="Reviews" list={ReviewList} edit={ReviewEdit} create={ReviewCreate} />
                <Resource name="Roles" list={EnumList} edit={EnumEdit} create={EnumCreate} />
                {/*<Resource name="Rooms" list={EnumList} edit={EnumEdit} create={EnumCreate} />*/}
                <Resource name="Spectators" list={UserPlanList} create={UserPlanCreate} />
                </Admin>
        );
    }
}
