
import {is} from 'bpmn-js/lib/util/ModelUtil';
import extensionElementsImpl from './impl/extensionElements';

import entryFactory from '../../lib/factory/EntryFactory';
import cmdHelper from 'bpmn-js-properties-panel/lib/helper/CmdHelper';


export default function scriptProps(group, element, bpmnFactory, translate) {
	if (is(element, 'bpmn:ScriptTask')) {
		let textBox = entryFactory.textBox(translate, {
			id: 'script',
			label: 'Script',
			modelProperty: 'script'
		});
		group.entries.push(textBox);
	}
	else if (is(element, "bpmn:UserTask")) {
		let textBox = entryFactory.textBox(translate, {
			id: 'script',
			label: 'Script',
			description: 'Executed after task completion',
			modelProperty: 'text',
			get(elem, node) {
				let ee = extensionElementsImpl.getExtensionElement(elem, "wf:Script");
				return ee ? {text: ee.text} : {};
			},
			set(elem, values, node) {
				let ee = extensionElementsImpl.getOrCreateExtensionElement(elem, 'wf:Script', bpmnFactory);
				ee.commands.push(cmdHelper.updateBusinessObject(elem, ee.elem, { text: values.text }));
				return ee.commands;
			}
		});
		group.entries.push(textBox);
	}
};
