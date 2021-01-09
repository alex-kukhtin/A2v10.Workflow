
import entryFactory from 'bpmn-js-properties-panel/lib/factory/EntryFactory';

import elementHelper from 'bpmn-js-properties-panel/lib/helper/ElementHelper';
import cmdHelper from 'bpmn-js-properties-panel/lib/helper/CmdHelper';

import { is, getBusinessObject } from 'bpmn-js/lib/util/ModelUtil';


function generateValueId() {
	return utils.nextId('Value_');
}

console.dir(elementHelper, cmdHelper, getBusinessObject );

export default function addProcessEntries(group, element, translate) {

	if (is(element, 'bpmn:Process') || is(element, 'bpmn:SubProcess')) {
		group.entries.push(entryFactory.table(translate, {
			id: 'variable',
			description: 'Activity variables',
			labels: ['name', 'direction'],
			modelProperties: ['name', 'direction'],
			getElements: (elem, node) => {
				console.dir({ s: 'get elements', elem, node });
				return [{ id:'id1', name: 'name1', direction: 'dir1' }, {id: 'id2', name: 'name2', direction: 'dir2'}];
			},
			addElement: (elem, node) => {
				console.dir('add element', elem, node);
				let commands = [];
				let bobj = getBusinessObject(elem);

				var extensionElements = bobj.get('extensionElements');
				if (!extensionElements) {
					//extensionElements = elementHelper.createElement('bpmn:ExtensionElements', { values: [] }, bo, bpmnFactory);
					//commands.push(cmdHelper.updateBusinessObject(element, bo, { extensionElements: extensionElements }));
				}

				console.dir({ s: 'add element', elem, node, bobj, extensionElements });
				return cmdHelper.addElementsTolist(elem, bobj, 'list_prop_name', [{}]);
			},
			removeElement: (elem, node, idx) => {
				console.dir('remove element', elem, node);
			},
			updateElement: (elem, value, node, idx) => {
				console.dir('update element', elem, value, node, idx);
			},
			validate: (elem, value, node, idx) => {
				console.dir('validate', elem, value, node, idx);
			}
		}));
	}
}