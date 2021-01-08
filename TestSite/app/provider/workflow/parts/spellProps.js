﻿
import entryFactory from 'bpmn-js-properties-panel/lib/factory/EntryFactory';
import {is} from 'bpmn-js/lib/util/ModelUtil';


export default function (group, element, translate) {

	// Only return an entry, if the currently selected
	// element is a start event.

	console.dir(entryFactory);

	if (is(element, 'bpmn:StartEvent')) {
		group.entries.push(entryFactory.textField(translate, {
			id: 'spell_id',
			description: 'Apply a black magic spell',
			label: 'Spell_label',
			modelProperty: 'spell_model_prop'
		}));
	}
}