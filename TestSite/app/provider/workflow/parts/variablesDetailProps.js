
import entryFactory from 'bpmn-js-properties-panel/lib/factory/EntryFactory';
import {is} from 'bpmn-js/lib/util/ModelUtil';

import extensionElementsImpl from './impl/extensionElements';
import cmdHelper from 'bpmn-js-properties-panel/lib/helper/CmdHelper';

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

	function setValue(prop, elem, values, node) {
		let vars = extensionElementsImpl.getSelectedVariableObject(node, elem);
		if (!vars) return;
		let update = {}
		update[prop] = values[prop];
		return cmdHelper.updateBusinessObject(elem, vars, update);
	}

	group.entries.push(entryFactory.textField(translate, {
		id: 'var_name',
		label: 'Name',
		modelProperty: 'Name',
		get(elem, node) {
			return extensionElementsImpl.getSelectedVariableObject(node, elem) || {};
		},
		set(elem, values, node) {
			return setValue('Name', elem, values, node);
		}
	}));

	group.entries.push(entryFactory.selectBox(translate, {
		id: 'var_type',
		label: 'Type',
		selectOptions: VAR_TYPE_OPTIONS,
		modelProperty: 'Type',
		get(elem, node) {
			return extensionElementsImpl.getSelectedVariableObject(node, elem) || {};
		},
		set(elem, values, node) {
			return setValue('Type', elem, values, node);
		}
	}));

	group.entries.push(entryFactory.selectBox(translate, {
		id: 'var_dir',
		label: 'Direction',
		selectOptions: VAR_DIR_OPTIONS,
		modelProperty: 'Dir',
		get(elem, node) {
			return extensionElementsImpl.getSelectedVariableObject(node, elem) || {};
		},
		set(elem, values, node) {
			return setValue('Dir', elem, values, node);
		}
	}));
}