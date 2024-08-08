namespace SlotTemplate
{

    public interface IDBCollectionReceiver
    {

        void OnDBsAssignedByDBCollection(GameDBManager.DBCollection dbCollection);
        void OnDBsAssignedByDBCollectionIncludeEmpty(GameDBManager.DBCollection dbCollection);

    }
}
