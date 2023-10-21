import * as React from "react";
import {
    List,
    Datagrid,
    TextField,
    ReferenceField,
    Edit,
    Create,
    SimpleForm,
    ReferenceInput,
    SelectInput,
    TextInput,
    Filter
} from 'react-admin';

export const ParticipantList = props => (
    <List filters={<ParticipantFilter/> } {...props}>
        <Datagrid rowClick="edit">
            <TextField source="id" />
            <ReferenceField source="UserId" reference="AppUsers"><TextField source="Username" /></ReferenceField>
            <ReferenceField source="RoomId" reference="Rooms"><TextField source="Name" /></ReferenceField>
        </Datagrid>
    </List>
);

export const ParticipantEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput disabled source="id" />
            <ReferenceInput source="UserId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="RoomId" reference="Rooms"><SelectInput optionText="Name" /></ReferenceInput>
        </SimpleForm>
    </Edit>
);

export const ParticipantCreate = props => (
    <Create {...props}>
        <SimpleForm>
            <ReferenceInput source="UserId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="RoomId" reference="Rooms"><SelectInput optionText="Name" /></ReferenceInput>
        </SimpleForm>
    </Create>
);

export const ParticipantFilter = (props) => (
    <Filter {...props}>
        <TextInput label="Search" source="q" alwaysOn />
    </Filter>
);