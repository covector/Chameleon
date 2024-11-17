using UnityEngine;
using static Utils;

public class Tutorial : MonoBehaviour
{
    // tutorial flags
    public static bool inProgress { get; private set; }
    public static bool lockPickUp { get; private set; }
    public static bool waitingScanner { get; private set; }
    public static bool waitingLocateItem { get; private set; }
    public static bool waitingPickup { get; private set; }
    public static bool waitingPickupFew { get; private set; }
    public static bool inFirstEncounter { get; private set; }
    public static bool waitingTurn { get; private set; }
    public static bool lockTurn { get; private set; }
    public static bool waitingLookAt { get; private set; }
    public static bool waitingBackOff { get; private set; }

    private void Start()
    {
        OffAllFlags();
        if (GameSettings.includeTutorial)
        {
            inProgress = true;
            lockPickUp = true;
        }
    }

    private static void OffAllFlags()
    {
        inProgress = false;
        lockPickUp = false;
        waitingScanner = false;
        waitingLocateItem = false;
        waitingPickup = false;
        waitingPickupFew = false;
        inFirstEncounter = false;
        waitingTurn = false;
        lockTurn = false;
        waitingLookAt = false;
        waitingBackOff = false;
    }

    public static void PlayTutorialDelay(MonoBehaviour mb, float delay)
    {
        inProgress = true;
        lockPickUp = true;
        RunDelay(mb, () => PlayTutorial(), delay);
    }

    public static void PlayTutorial()
    {
        inProgress = true;
        lockPickUp = true;
        Dialogue d = FindFirstObjectByType<Dialogue>();
        d.Say("Hello? Testing testing. Can you hear me?", () =>
        d.Say("Great! The receiver doesn't always pick up our signal you know.", () =>
        d.Say("I forgot to introduce myself, I am commander @@.\nI will be your guide throughout the night.\nWe look forward to working with you.", () =>
        d.Say("Are you alright? You sounded a little nervous.\nWell, it's natural since this is your first night ha ha.", () =>
        d.Say("It will be completely safe... As long you follow all our protocols, you will be fine.", () =>
        d.Say($"Now, We will need you to test your radar scanner. Press -{ToReadable(PlayerOptions.instance.KeyBinds["Scanner"])}- to switch on/off the device.", manualNext: true, callback:() =>
        { waitingScanner = true; }
        ))))));  // Callback hell lol
    }

    public static void Scanner()
    {
        if (!waitingScanner) { return; }
        waitingScanner = false;
        Dialogue d = FindFirstObjectByType<Dialogue>();
        d.Say("The green dots on the display indicate the location of the items you will collect.\nWalk around a bit if you don't see any dots.", () =>
        d.Say("Follow the scanner, and head toward the nearest dot.", manualNext: true, callback: () =>
        { waitingLocateItem = true; }
        ));
    }

    public static void LocatedItem()
    {
        if (!waitingLocateItem) { return; }
        waitingLocateItem = false;
        Dialogue d = FindFirstObjectByType<Dialogue>();
        d.Say($"Amazing! You have found your first item.", () =>
        d.Say($"We will need you to look at the device-like item on the ground. And press -{ToReadable(PlayerOptions.instance.KeyBinds["PickUp"])}- to pick it up.",
        manualNext: true, callback: () =>
        { waitingPickup = true; lockPickUp = false; }
        ));
    }

    public static void PickUp()
    {
        if (waitingPickup)  // pick up first
        {
            waitingPickup = false;
            Dialogue d = FindFirstObjectByType<Dialogue>();
            d.Say("Splendid! I have never seen someone collect an item this early on their first night.", () =>
            d.Say("You might have talent for this ha ha.", () =>
            d.Say("Remember to collect as much as possible, as they are valuable assets to our organization. ", () =>
            d.Say("We will be here should you need any further assistance.", () =>
            { d.HideAll(); waitingPickupFew = true; }
            ))));
        } else if (waitingPickupFew)    // pick up 4
        {
            if (FindFirstObjectByType<ItemCounter>().GetCount() < 4) { return; }
            waitingPickupFew = false;
            Dialogue d = FindFirstObjectByType<Dialogue>();
            d.Say("Looks like you are getting the hang of it. Keep it up!", () =>
            { d.HideAll(); RunDelay(() => StartFirstEncounter(), 10f); }
            );
        }
    }

    private static void StartFirstEncounter()
    {
        inFirstEncounter = true;
        // lock rotation + position
        Dialogue d = FindFirstObjectByType<Dialogue>();
        FindFirstObjectByType<RadarScanner>().TurnOffRadar();
        d.Say("Turn around now!! Do you hear me? \nYou need to turn around right now!!", manualNext: true, callback: () =>
        {
            // unlock rotation
            waitingTurn = true;
        }
        );
    }

    public static void TurnAround()
    {
        if (!waitingTurn) { return; }
        waitingTurn = false;
        Dialogue d = FindFirstObjectByType<Dialogue>();
        d.Say("Can you see it? Do you not see it?", manualNext: true, callback: () =>
        {
            waitingLookAt = true;
        }
        );
    }

    public static void LookAt()
    {
        if (!waitingLookAt) { return; }
        waitingLookAt = false;
        lockTurn = true;
        // lock rotation
        TutorialDialogue d = FindFirstObjectByType<TutorialDialogue>();
        d.Say("Oh man, it must have shape-shifted into a rock or something.", () =>
        d.Say("Okay... I need you to listen to me carefully.", () =>
        d.Say("I need you to continue staring at it.\nThen slowly walk backward.", manualNext: true, callback: () =>
        {
            // unlock position back only
            waitingBackOff = true;
        }
        )));
    }

    public static void BackedOff()
    {
        if (!waitingBackOff) { return; }
        waitingBackOff = false;
        inFirstEncounter = false;
        // unlock everything
        lockTurn = false;
        TutorialDialogue d = FindFirstObjectByType<TutorialDialogue>();
        d.Say("Phew, that was close. I forgot to mention earlier it is known that a monster lurks in these woods.", () =>
        d.Say("It can shape-shift and blends in with the environment.", () =>
        d.Say("But don't worry, we are equipped with state-of-the-art quantum surveillance radar.\nLocating this monster will be a piece of cake.", () =>
        d.Say("We will just alert you when it gets close.", () =>
        {
            d.useGlitch1 = true;
            d.Say("Luckily you are able to pick up o@r s#gnal-", () =>
            {
                d.useGlitch1 = true;
                d.Say("#t w@ld ahve ###been ba@@d #f...", () =>
                {
                    d.HideAll(); OffAllFlags();
                });
            });
        }))));  // holy fuck the callback goes CRAZY
    }
}
