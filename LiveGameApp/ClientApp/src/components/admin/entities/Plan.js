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
    Filter,
    NumberField,
    NumberInput,
} from 'react-admin';
import { DateField, DateTimeInput } from '../custom/CustomDates';

export const PlanList = props => (
    <List filters={<PlanFilter />} {...props}>
        <Datagrid rowClick="edit">
            <TextField source="id" />
            <TextField source="Name" />
            <DateField source="Datetime" />
            <TextField source="Location" />
            <NumberField source="MaxPlayers" className="d-flex justify-content-center"/>
            <NumberField source="MaxSpectators" className="d-flex justify-content-center"/>
            <ReferenceField source="HostUserId" reference="AppUsers"><TextField source="Username" /></ReferenceField>
            <ReferenceField source="GameId" reference="Games"><TextField source="Name" /></ReferenceField>
            <ReferenceField source="TypeId" reference="PlanTypes"><TextField source="Name" /></ReferenceField>
            <ReferenceField source="PrivacyTypeId" reference="PrivacyTypes"><TextField source="Name" /></ReferenceField>
            <EditButton />
        </Datagrid>
    </List>
);

export const PlanEdit = props => (
    <Edit {...props}>
        <SimpleForm>
            <TextInput disabled source="id" />
            <TextInput source="Name" />
            <DateTimeInput source="Datetime" />
            <TextInput source="Location" />
            <NumberInput source="MaxPlayers" />
            <NumberInput source="MaxSpectators" />
            <ReferenceInput source="HostUserId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="GameId" reference="Games"><SelectInput optionText="Name" /></ReferenceInput>
            <ReferenceInput source="TypeId" reference="PlanTypes"><SelectInput optionText="Name" /></ReferenceInput>
            <ReferenceInput source="PrivacyTypeId" reference="PrivacyTypes"><SelectInput optionText="Name" /></ReferenceInput>
        </SimpleForm>
    </Edit>
);

export const PlanCreate = props => (
    <Create {...props}>
        <SimpleForm>
            <TextInput source="Name" />
            <DateTimeInput source="Datetime" />
            <TextInput source="Location" />
            <NumberInput source="MaxPlayers" />
            <NumberInput source="MaxSpectators" />
            <ReferenceInput source="HostUserId" reference="AppUsers"><SelectInput optionText="Username" /></ReferenceInput>
            <ReferenceInput source="GameId" reference="Games"><SelectInput optionText="Name" /></ReferenceInput>
            <ReferenceInput source="TypeId" reference="PlanTypes"><SelectInput optionText="Name" /></ReferenceInput>
            <ReferenceInput source="PrivacyTypeId" reference="PrivacyTypes"><SelectInput optionText="Name" /></ReferenceInput>
        </SimpleForm>
    </Create>
);

export const PlanFilter = (props) => (
    <Filter {...props}>
        <TextInput label="Search" source="q" alwaysOn />
        <ReferenceInput label="Host" source="HostUserId" reference="AppUsers" allowEmpty>
            <SelectInput optionText="Username" />
        </ReferenceInput>
    </Filter>
);