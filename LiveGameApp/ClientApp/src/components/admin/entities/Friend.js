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

export const FriendList = props => (
    <List filters={<FriendFilter/> } {...props}>
        <Datagrid rowClick="edit">
            <TextField source="id" />
            <ReferenceField source="UserLowId" reference="AppUsers"><TextField source="Username" /></ReferenceField>
            <ReferenceField source="UserHighId" reference="AppUsers"><TextField source="Username" /></ReferenceField>
        </Datagrid>
    </List>
);

export const FriendEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput disabled source="id" />
            <ReferenceInput source="UserLowId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="UserHighId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
        </SimpleForm>
    </Edit>
);

export const FriendCreate = props => (
    <Create {...props}>
        <SimpleForm>
            <ReferenceInput source="UserLowId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="UserHighId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
        </SimpleForm>
    </Create>
);

export const FriendFilter = (props) => (
    <Filter {...props}>
        <TextInput label="Search" source="q" alwaysOn />
    </Filter>
);