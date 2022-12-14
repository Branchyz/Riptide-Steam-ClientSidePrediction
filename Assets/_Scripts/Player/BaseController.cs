using UnityEngine;

public class BaseController : PhysicsBody 
{
    protected InputPayload[] inputPayloadBuffer = new InputPayload[ServerSettings.BUFFER_SIZE];
    //*THIS IS NEEDED AS WHEN THE GAME STARTS, THE GRAVITY IS APPLIED TO THE SERVER PLAYER
    //*EVEN WHEN THERE HAS BEEN NO INPUT LEADING TO AN INCONSISTENCY IN STATE.
    protected bool canSimulate = true;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        DoStep += Step;
        for(int i = 0; i < inputPayloadBuffer.Length; i++)
            inputPayloadBuffer[i] = new InputPayload();
    }

    private void Step(object sender, StepEventArgs e)
	{
        if(!canSimulate) return;

        if(e.bodyToIgnore != null)
			if(e.bodyToIgnore.gameObject == this.gameObject)
				return;
        Move(inputPayloadBuffer[(localTick - 1) % ServerSettings.BUFFER_SIZE]);
	}

    protected void Move(InputPayload inputPayload)
	{
		Vector3 wishDir = new Vector3(inputPayload.xInput, 0, inputPayload.zInput);
        rb.velocity = Movement.Gravity(rb.velocity, ServerSettings.TICK_DT);
		rb.velocity = Movement.Accelerate(rb.velocity, wishDir, ServerSettings.TICK_DT);
	}

    protected override void OnDisable()
    {
        base.OnDisable();
        DoStep -= Step;
    }
}