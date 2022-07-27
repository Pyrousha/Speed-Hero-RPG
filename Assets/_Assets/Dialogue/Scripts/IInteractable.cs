public interface IInteractable
{
    public int Priority { get; set; }

    void TryInteract(HeroDialogueInteract player);
}
