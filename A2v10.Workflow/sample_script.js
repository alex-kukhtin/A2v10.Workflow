
/*
<Sequence Ref="Ref0">
	<Variables>
		<Variable Name="Arg" Dir="In" Value="5" Type="Number">
		<Variable Name="Result" Dir="Out" Type="Number">
	</Variables>
	<If Condition = "Arg.x > 5" Ref="RefIf">
		<If.Then>
			<Code Ref="RefThen" Script="Result='x > 5'"/>
		</If.Then>
	<If.Else>
		<Code Ref = "RefElse" Script="Result = 'x <= 5'">
	</If.Else>
</Sequence>
*/

// Arg = {}
// Result = {}

"use strict";

let wf = function () {

	/* VARIABLES */
	let Arg;
	let Result;

	const __fmap__ = {};

	const CONST1 = 'CONST1';
	const CONST2 = 'CONST2';

	let Outer1;
	let Outer2;

	// in arguments
	__fmap__['Ref0.Arg'] = (_obj_) => { Arg = _obj_.Arg; }

	__fmap__['Ref0.Store'] = () => {
		return { Outer1: Outer1, Outer2: Outer2, Arg: Arg, Result: Result };
	};

	__fmap__['Ref0.Restore'] = (_obj_) => {
		Outer1 = _obj_.Outer1, Outer2 = _obj.Outer2, Arg = _obj_.Arg, Result = _obj_.Result;
	};

	// out arguments
	__fmap__['Ref0.Result'] = () => { return {Result: Result};}

	(function () {

		let Inner1 = 7;
		let Inner2 = 'test';

		__fmap__['Ref0.Store'] = () => {
			return { Inner1: Inner1, Inner2: Inner2};
		};

		__fmap__['Ref0.Restore'] = (r) => {
			Inner1 = r.Inner1, Inner2 = r.Inner2;
		};

		__fmap__['RefIf.Condition'] = () => { Arg.x > 5; };

		(function () {
			__fmap__['RefThen.Script'] = () => {
				console.dir({ CONST1, CONST2, Outer1, Outer2, Inner1, Inner2 });
				Result = 'x > 5';
			};
		})();
	})();

	return __fmap__;
};

// это вызов в самом начале
var r = wf();
r['Workflow.Arg']({ x: 7 });

console.dir(r);

r['RefThen.Script']();

let state = {
	'Workflow': r['Workflow.Store'](),
	'Ref0': r['Ref0.Store']()
};

console.dir(state);