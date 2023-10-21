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

export const UserPlanList = props => (
    <List filters={<UserPlanFilter />} {...props}>
        <Datagrid rowClick="edit">
            <TextField source="id" />
            <ReferenceField source="UserId" reference="AppUsers"><TextField source="Username" /></ReferenceField>
            <ReferenceField source="PlanId" reference="Plans"><TextField source="Name" /></ReferenceField>
        </Datagrid>
    </List>
);

export const UserPlanEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput disabled source="id" />
            <ReferenceInput source="UserId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="PlanId" reference="Plans"><SelectInput optionText="Name" /></ReferenceInput>
        </SimpleForm>
    </Edit>
);

export const UserPlanCreate = props => (
    <Create {...props}>
        <SimpleForm>
            <ReferenceInput source="UserId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="PlanId" reference="Plans"><SelectInput optionText="Name" /></ReferenceInput>
        </SimpleForm>
    </Create>
);

export const UserPlanFilter = (props) => (
    <Filter {...props}>
        <TextInput label="Search" source="q" alwaysOn />
    </Filter>
);