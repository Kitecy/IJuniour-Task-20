public class ResourceSpawner : Spawner<Resource>
{
    protected override void OnGetObject(Resource obj)
    {
        base.OnGetObject(obj);
        obj.SetSpawner(this);
    }
}
