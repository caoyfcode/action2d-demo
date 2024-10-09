using System;
using System.Collections.Generic;


public class BTBuilder
{
    private BTNode rootNode = null;
    // private BTNode curNode = null;
    private readonly Stack<BTComposite> parentNodeStack = new();

    public BehaviourTree Build()
    {
        var tree = new BehaviourTree(rootNode);
        rootNode = null;
        parentNodeStack.Clear();
        return tree;
    }

    public BTNode BuildNode()
    {
        var node = rootNode;
        rootNode = null;
        parentNodeStack.Clear();
        return node;
    }

    // 放置自定义叶子结点或子树
    public BTBuilder Node(BTNode node)
    {
        // curNode = node;
        if (rootNode == null)
        {
            rootNode = node;
        }
        else if (parentNodeStack.Count > 0)
        {
            parentNodeStack.Peek().AddChild(node);
        }
        else
        {
            throw new Exception("Invalid tree structure");
        }
        return this;
    }

    public BTBuilder Composite(BTComposite node)
    {
        Node(node);
        parentNodeStack.Push(node);
        return this;
    }

    public BTBuilder End()
    {
        if (parentNodeStack.Count > 0)
        {
            parentNodeStack.Pop();
        }
        return this;
    }

    public BTBuilder Decorator(BTDecorator node)
    {
        return Composite(node);
    }

    public BTBuilder Action(Func<BTNodeStatus> action, Action onActivate = null, Action onDeactivate = null)
    {
        return Node(new BTAction(action, onActivate, onDeactivate));
    }

    public BTBuilder Condition(Func<bool> condition)
    {
        return Node(new BTCondition(condition));
    }

    public BTBuilder WaitUntil(Func<bool> condition, Action onActivate = null, Action onDeactivate = null)
    {
        return Node(new BTWaitUntil(condition, onActivate, onDeactivate));
    }

    public BTBuilder Inverter()
    {
        return Decorator(new BTInverter());
    }

    public BTBuilder Fallback()
    {
        return Composite(new BTFallback());
    }

    public BTBuilder Sequence()
    {
        return Composite(new BTSequence());
    }

    public BTBuilder Parallel()
    {
        return Composite(new BTParallel());
    }
}

public class BehaviourTree
{
    public readonly BTNode rootNode;
    public BTNodeStatus Status { get; private set; } = BTNodeStatus.Success;

    public BehaviourTree(BTNode rootNode)
    {
        this.rootNode = rootNode;
    }

    public BTNodeStatus Update()
    {
        Status = rootNode.Execute();
        return Status;
    }

}

public enum BTNodeStatus
{
    Success,
    Failure,
    Running,
}

public abstract class BTNode
{
    public BTNode Parent = null;
    public BTNodeStatus Status { get; protected set; } = BTNodeStatus.Success;
    protected bool _isActive = false;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value)
                return;
            _isActive = value;
            if (_isActive)
                OnActivate();
            else
                OnDeactivate();
        }
    }

    public BTNodeStatus Execute()
    {
        if (!IsActive)
        {
            IsActive = true;
        }
        Status = OnExecute();
        if (Status != BTNodeStatus.Running)
        {
            IsActive = false;
        }
        return Status;
    }

    public abstract BTNodeStatus OnExecute();
    public virtual void OnActivate() { }
    public virtual void OnDeactivate() { }
}

public abstract class BTComposite : BTNode
{
    public List<BTNode> Children { get; protected set; }
    public int runningChildIndex = -1; // 子类维护, 当 Status 为 Running 时有意义

    public BTComposite(params BTNode[] children)
    {
        Children = new List<BTNode>(children);
        foreach (var child in Children)
        {
            child.Parent = this;
        }
        runningChildIndex = -1;
    }

    public virtual void AddChild(BTNode child)
    {
        Children.Add(child);
        child.Parent = this;
    }
}

public abstract class BTDecorator : BTComposite
{
    public BTNode Child => Children[0];

    public BTDecorator() : base()
    {
    }

    public BTDecorator(BTNode child) : base(child)
    {
    }

    public override void AddChild(BTNode child)
    {
        if (Children.Count == 0)
            base.AddChild(child);
    }

    public void SetChild(BTNode child)
    {
        Children.Clear();
        base.AddChild(child);
    }
}

# region Leaf Nodes
public class BTAction : BTNode
{
    private readonly Func<BTNodeStatus> action;
    private readonly Action onActivate;
    private readonly Action onDeactivate;

    public BTAction(Func<BTNodeStatus> action, Action onActivate = null, Action onDeactivate = null)
    {
        this.action = action;
        this.onActivate = onActivate;
        this.onDeactivate = onDeactivate;
    }

    public override BTNodeStatus OnExecute()
    {
        return action?.Invoke() ?? BTNodeStatus.Success;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        onActivate?.Invoke();
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        onDeactivate?.Invoke();
    }
}

public class BTCondition : BTNode
{
    private readonly Func<bool> condition;

    public BTCondition(Func<bool> condition)
    {
        this.condition = condition;
    }

    public override BTNodeStatus OnExecute()
    {
        bool result = condition?.Invoke() ?? true;
        return result ? BTNodeStatus.Success : BTNodeStatus.Failure;
    }
}

public class BTWaitUntil : BTNode
{
    private readonly Func<bool> condition;
    private readonly Action onActivate;
    private readonly Action onDeactivate;

    public BTWaitUntil(Func<bool> condition, Action onActivate = null, Action onDeactivate = null)
    {
        this.condition = condition;
        this.onActivate = onActivate;
        this.onDeactivate = onDeactivate;
    }

    public override BTNodeStatus OnExecute()
    {
        bool result = condition?.Invoke() ?? true;
        return result ? BTNodeStatus.Success : BTNodeStatus.Running;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        onActivate?.Invoke();
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        onDeactivate?.Invoke();
    }
}
#endregion

#region Decorators
public class BTInverter : BTDecorator
{
    public BTInverter() : base()
    {
    }
    public BTInverter(BTNode child) : base(child)
    {
    }

    public override BTNodeStatus OnExecute()
    {
        if (Children.Count == 0)
        {
            return BTNodeStatus.Success;
        }
        var status = Child.Execute();
        if (status == BTNodeStatus.Success)
        {
            return BTNodeStatus.Failure;
        }
        else if (status == BTNodeStatus.Failure)
        {
            return BTNodeStatus.Success;
        }
        else
        {
            return status;
        }
    }
}
#endregion

#region Composites
public class BTFallback : BTComposite
{
    public BTFallback(params BTNode[] children) : base(children)
    {
    }

    public override BTNodeStatus OnExecute()
    {
        if (Children.Count == 0)
        {
            return BTNodeStatus.Success;
        }
        int startIndex = 0;
        if (Status == BTNodeStatus.Running) // resume
        {
            startIndex = runningChildIndex;
        }
        for (int i = startIndex; i < Children.Count; i++)
        {
            runningChildIndex = i;
            var status = Children[i].Execute();
            if (status != BTNodeStatus.Failure)
            {
                return status;
            }
        }
        return BTNodeStatus.Failure;
    }
}

public class BTSequence : BTComposite
{
    public BTSequence(params BTNode[] children) : base(children)
    {
    }

    public override BTNodeStatus OnExecute()
    {
        if (Children.Count == 0)
        {
            return BTNodeStatus.Success;
        }
        int startIndex = 0;
        if (Status == BTNodeStatus.Running) // resume
        {
            startIndex = runningChildIndex;
        }
        for (int i = startIndex; i < Children.Count; i++)
        {
            runningChildIndex = i;
            var status = Children[i].Execute();
            if (status != BTNodeStatus.Success)
            {
                return status;
            }
        }
        return BTNodeStatus.Success;
    }
}

public class BTParallel : BTComposite
{
    public int successNum = 0;
    public BTParallel(int successNum = 0, params BTNode[] children) : base(children)
    {
        this.successNum = successNum;
    }

    public override BTNodeStatus OnExecute()
    {
        if (Children.Count == 0)
        {
            return BTNodeStatus.Success;
        }
        int successCount = 0;
        int failureCount = 0;

        foreach (var child in Children)
        {
            var status = child.Execute();
            if (status == BTNodeStatus.Success)
            {
                successCount++;
            }
            else if (status == BTNodeStatus.Failure)
            {
                failureCount++;
            }
            
        }
        // 放在最末尾而不是提前结束的原因是需要保证上一帧为 Running 的结点下一帧被执行到
        // FIXME 现在仍然有部分无法退出的 bug
        if (successCount + failureCount < Children.Count)
        {
            return BTNodeStatus.Running;
        }
        if (successCount >= successNum)
        {
            return BTNodeStatus.Success;
        }
        else
        {
            return BTNodeStatus.Failure;
        }
    }
}
#endregion