import * as React from "react";
import {
    List,
    Datagrid,
    TextField,
    ReferenceField,
    EditButton,
    Edit,
    Create,
    SimpleForm,
    ReferenceInput,
    SelectInput,
    TextInput,
    Filter
} from 'react-admin';
import { DateField, DateTimeInput } from '../custom/CustomDates';

export const MessageList = props => (
    <List filters={<MessageFilter/>} {...props}>
        <Datagrid rowClick="edit">
            <TextField source="id" />
            <TextField source="Content" />
            <DateField source="Datetime" />
            <ReferenceField source="UserId" reference="AppUsers"><TextField source="Username" /></ReferenceField>
            <ReferenceField source="RoomId" reference="Rooms"><TextField source="Name" /></ReferenceField>
            <EditButton/>
        </Datagrid>
    </List>
);

export const MessageEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput disabled source="id" />
            <TextInput source="Content" />
            <DateTimeInput source="Datetime" />
            <ReferenceInput source="UserId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="RoomId" reference="Rooms"><SelectInput optionText="Name" /></ReferenceInput>
        </SimpleForm>
    </Edit>
);

export const MessageCreate = props => (
    <Create {...props}>
        <SimpleForm>
            <TextInput source="Content" />
            <DateTimeInput source="Datetime" />
            <ReferenceInput source="UserId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="RoomId" reference="Rooms"><SelectInput optionText="Name" /></ReferenceInput>
        </SimpleForm>
    </Create>
);

export const MessageFilter = (props) => (
    <Filter {...props}>
        <TextInput label="Search" source="q" alwaysOn />
    </Filter>
);