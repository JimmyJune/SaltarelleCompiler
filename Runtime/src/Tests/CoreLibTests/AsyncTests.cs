﻿using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using System.Testing;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibTests {
	[TestFixture]
	public class AsyncTests {
#if !NO_ASYNC
		[AsyncTest]
		public void AsyncVoid() {
			int state = 0;
			var tcs = new TaskCompletionSource<int>();
			Task task = tcs.Task;

			Action someMethod = async () => {
				state = 1;
				await task;
				state = 2;
			};
			someMethod();
			Assert.AreEqual(state, 1, "Async method should start running after being invoked");

			Window.SetTimeout(() => {
				Assert.AreEqual(state, 1, "Async method should not continue past point 1 until task is finished");
			}, 100);

			Window.SetTimeout(() => {
				tcs.SetResult(0);
			}, 200);

			Window.SetTimeout(() => {
				Assert.AreEqual(state, 2, "Async method should finish after the task is finished");
				QUnit.Start();
			}, 300);
		}

		[AsyncTest]
		public void AsyncTask() {
			int state = 0;
			var tcs = new TaskCompletionSource<int>();
			Task task = tcs.Task;

			Func<Task> someMethod = async () => {
				state = 1;
				await task;
				state = 2;
			};
			Task asyncTask = someMethod();
			Assert.AreEqual(asyncTask.Status, TaskStatus.Running, "asyncTask should be running immediately");
			Assert.AreEqual(state, 1, "Async method should start running after being invoked");

			Window.SetTimeout(() => {
				Assert.AreEqual(asyncTask.Status, TaskStatus.Running, "asyncTask should be running before awaited task is finished");
				Assert.AreEqual(state, 1, "Async method should not continue past point 1 until task is finished");
			}, 100);

			Window.SetTimeout(() => {
				tcs.SetResult(0);
			}, 200);

			Window.SetTimeout(() => {
				Assert.AreEqual(asyncTask.Status, TaskStatus.RanToCompletion, "asyncTask should run to completion");
				Assert.IsTrue(asyncTask.Exception == null, "asyncTask should not throw an exception");
				Assert.AreEqual(state, 2, "Async method should finish after the task is finished");
				QUnit.Start();
			}, 300);
		}

		[AsyncTest]
		public void AsyncTaskBodyThrowsException() {
			int state = 0;
			var tcs = new TaskCompletionSource<int>();
			Task task = tcs.Task;
			var ex = new Exception("Some text");

			Func<Task> someMethod = async () => {
				state = 1;
				await task;
				state = 2;
				throw ex;
			};
			Task asyncTask = someMethod();
			Assert.AreEqual(asyncTask.Status, TaskStatus.Running, "asyncTask should be running immediately");
			Assert.AreEqual(state, 1, "Async method should start running after being invoked");

			Window.SetTimeout(() => {
				Assert.AreEqual(asyncTask.Status, TaskStatus.Running, "asyncTask should be running before awaited task is finished");
				Assert.AreEqual(state, 1, "Async method should not continue past point 1 until task is finished");
			}, 100);

			Window.SetTimeout(() => {
				tcs.SetResult(0);
			}, 200);

			Window.SetTimeout(() => {
				Assert.AreEqual(asyncTask.Status, TaskStatus.Faulted, "asyncTask should fault");
				Assert.IsTrue(asyncTask.Exception != null, "asyncTask should have an exception");
				Assert.IsTrue(asyncTask.Exception.InnerExceptions[0] == ex, "asyncTask should throw the correct exception");
				Assert.AreEqual(state, 2, "Async method should finish after the task is finished");
				QUnit.Start();
			}, 300);
		}

		[AsyncTest]
		public void AwaitTaskThatFaults() {
			int state = 0;
			var tcs = new TaskCompletionSource<int>();
			Task task = tcs.Task;
			var ex = new Exception("Some text");

			Func<Task> someMethod = async () => {
				state = 1;
				await task;
				state = 2;
			};
			Task asyncTask = someMethod();
			Assert.AreEqual(asyncTask.Status, TaskStatus.Running, "asyncTask should be running immediately");
			Assert.AreEqual(state, 1, "Async method should start running after being invoked");

			Window.SetTimeout(() => {
				Assert.AreEqual(asyncTask.Status, TaskStatus.Running, "asyncTask should be running before awaited task is finished");
				Assert.AreEqual(state, 1, "Async method should not continue past point 1 until task is finished");
			}, 100);

			Window.SetTimeout(() => {
				tcs.SetException(ex);
			}, 200);

			Window.SetTimeout(() => {
				Assert.AreEqual(asyncTask.Status, TaskStatus.Faulted, "asyncTask should fault");
				Assert.IsTrue(asyncTask.Exception != null, "asyncTask should have an exception");
				Assert.IsTrue(asyncTask.Exception.InnerExceptions[0] == ex, "asyncTask should throw the correct exception");
				Assert.AreEqual(state, 1, "Async method should not have reach anything after the faulting await");
				QUnit.Start();
			}, 300);
		}

		[AsyncTest]
		public void AsyncTaskThatReturnsValue() {
			int state = 0;
			var tcs = new TaskCompletionSource<int>();
			Task task = tcs.Task;

			Func<Task<int>> someMethod = async () => {
				state = 1;
				await task;
				state = 2;
				return 42;
			};
			Task<int> asyncTask = someMethod();
			Assert.AreEqual(asyncTask.Status, TaskStatus.Running, "asyncTask should be running immediately");
			Assert.AreEqual(state, 1, "Async method should start running after being invoked");

			Window.SetTimeout(() => {
				Assert.AreEqual(asyncTask.Status, TaskStatus.Running, "asyncTask should be running before awaited task is finished");
				Assert.AreEqual(state, 1, "Async method should not continue past point 1 until task is finished");
			}, 100);

			Window.SetTimeout(() => {
				tcs.SetResult(0);
			}, 200);

			Window.SetTimeout(() => {
				Assert.AreEqual(asyncTask.Status, TaskStatus.RanToCompletion, "asyncTask should run to completion");
				Assert.IsTrue(asyncTask.Exception == null, "asyncTask should not throw an exception");
				Assert.AreEqual(state, 2, "Async method should finish after the task is finished");
				Assert.AreEqual(asyncTask.Result, 42, "Result should be correct");
				QUnit.Start();
			}, 300);
		}
#endif
	}
}
