"use strict";
(function () {
	let __fmap__ = {};
	(function () {
		let X;
		let R;

		__fmap__.Ref0 = {
			Arguments: function (_arg_) { X = _arg_.X; },
			Result: function () { return { R: R }; },
			Store: function () { return { X: X, R: R }; },
			Restore: function (_arg_) { X = _arg_.X; R = _arg_.R; }
		};

		__fmap__.Ref1 = {
			Script: function () { X = X + 1; }
		};

		__fmap__.Ref2 = {
			Script: function () { X = X + 1; }
		};

		__fmap__.Ref3 = {
			Script: function () { R = X; }
		};
	})();
	return __fmap__;
})();