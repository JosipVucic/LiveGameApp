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

export const InvitationList = props => (
    <List filters={<InvitationFilter />} {...props}>
        <Datagrid rowClick="edit">
            <TextField source="id" />
            <TextField source="Message" />
            <ReferenceField source="RecipientId" reference="AppUsers"><TextField source="Username" /></ReferenceField>
            <ReferenceField source="TypeId" reference="InviteRequestTypes"><TextField source="Name" /></ReferenceField>
            <ReferenceField source="PlanId" reference="Plans"><TextField source="Name" /></ReferenceField>
            <EditButton />
        </Datagrid>
    </List>
);

export const InvitationEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput disabled source="id" />
            <TextInput source="Message" />
            <ReferenceInput source="RecipientId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="TypeId" reference="InviteRequestTypes"><SelectInput optionText="Name" /></ReferenceInput>
            <ReferenceInput source="PlanId" reference="Plans"><SelectInput optionText="Name" /></ReferenceInput>
        </SimpleForm>
    </Edit>
);

export const InvitationCreate = props => (
    <Create {...props}>
        <SimpleForm>
            <TextInput source="Message" />
            <ReferenceInput source="RecipientId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="TypeId" reference="InviteRequestTypes"><SelectInput optionText="Name" /></ReferenceInput>
            <ReferenceInput source="PlanId" reference="Plans"><SelectInput optionText="Name" /></ReferenceInput>
        </SimpleForm>
    </Create>
);

export const InvitationFilter = (props) => (
    <Filter {...props}>
        <TextInput label="Search" source="q" alwaysOn />
    </Filter>
);