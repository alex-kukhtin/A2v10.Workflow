
import entryFactory from 'bpmn-js-properties-panel/lib/factory/EntryFactory';
import {is} from 'bpmn-js/lib/util/ModelUtil';

const VAR_TYPE_OPTIONS = [
	{ name: 'String', value: 'String' },
	{ name: 'Number', value: 'Number' },
	{ name: 'Object', value: 'Object' }
];

const VAR_DIR_OPTIONS = [
	{ name: 'In', value: 'In' },
	{ name: 'Out', value: 'Out' },
	{ name: 'InOut', value: 'InOut' },
	{ name: 'Local', value: 'Local' }
];

export default function VariablesDetailProps(group, element, translate) {

	group.entries.push(entryFactory.textField(translate, {
		id: 'var_name',
		label: 'Name',
		modelProperty: 'Name'
	}));

	group.entries.push(entryFactory.selectBox(translate, {
		id: 'var_type',
		label: 'Type',
		selectOptions: VAR_TYPE_OPTIONS,
		modelProperty: 'Type'
	}));

	group.entries.push(entryFactory.selectBox(translate, {
		id: 'var_dir',
		label: 'Direction',
		selectOptions: VAR_DIR_OPTIONS,
		modelProperty: 'Dir'
	}));
}