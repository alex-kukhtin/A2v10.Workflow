
import entryFactory from 'bpmn-js-properties-panel/lib/factory/EntryFactory';
import {is} from 'bpmn-js/lib/util/ModelUtil';


export default function scriptProps(group, element, translate) {
	if (is(element, 'bpmn:ScriptTask') || is(element, 'bpmn:UserTask')) {
		let textBox = entryFactory.textBox(translate, {
			id: 'script',
			label: 'Script',
			modelProperty: 'script'
		});
		console.dir(textBox);
		group.entries.push(textBox);
	}
};
