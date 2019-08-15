﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using magic.node;
using magic.signals.contracts;

namespace magic.lambda.branching
{
    [Slot(Name = "else")]
    public class Else : ISlot, IMeta
    {
        readonly ISignaler _signaler;

        public Else(IServiceProvider services)
        {
            _signaler = services.GetService(typeof(ISignaler)) as ISignaler;
        }

        public void Signal(Node input)
        {
            var previous = input.Previous;
            if (previous == null || (previous.Name != "if" && previous.Name != "else-if"))
                throw new ApplicationException("[else] must have an [if] or [else-if] before it");

            var evaluate = true;
            while (previous != null && (previous.Name == "if" || previous.Name == "else-if"))
            {
                if (previous.Children.First().Get<bool>())
                {
                    evaluate = false;
                    break;
                }
                previous = previous.Previous;
            }
            if (evaluate)
                _signaler.Signal("eval", input);
        }

        public IEnumerable<Node> GetArguments()
        {
            yield return new Node("*", "*");
        }
    }
}