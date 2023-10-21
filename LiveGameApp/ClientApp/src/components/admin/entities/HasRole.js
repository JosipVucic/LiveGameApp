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

export const HasRoleList = props => (
    <List filters={<HasRoleFilter/> } {...props}>
        <Datagrid rowClick="edit">
            <TextField source="id" />
            <ReferenceField source="UserId" reference="AppUsers"><TextField source="Username" /></ReferenceField>
            <ReferenceField source="RoleId" reference="Roles"><TextField source="Name" /></ReferenceField>
        </Datagrid>
    </List>
);

export const HasRoleEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput disabled source="id" />
            <ReferenceInput source="UserId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="RoleId" reference="Roles"><SelectInput optionText="Name" /></ReferenceInput>
        </SimpleForm>
    </Edit>
);

export const HasRoleCreate = props => (
    <Create {...props}>
        <SimpleForm>
            <ReferenceInput source="UserId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="RoleId" reference="Roles"><SelectInput optionText="Name" /></ReferenceInput>
        </SimpleForm>
    </Create>
);

export const HasRoleFilter = (props) => (
    <Filter {...props}>
        <TextInput label="Search" source="q" alwaysOn />
    </Filter>
);