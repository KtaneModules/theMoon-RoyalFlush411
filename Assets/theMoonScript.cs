using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using theMoon;

public class theMoonScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable[] buttons;
    public Light[] lights;

    //Light info
    public List<int> offLights = new List<int>();
    public List<String> offLightsName = new List<string>();
    public List<String> positions = new List<string>();
    private int startPosition = 0;
    private int serialDigitsTotal = 0;
    private int lightIndex = 0;
    private bool lightsPicked = false;
    private int lightCount = 0;

    //Sections
    private int dBatteries = 0;
    private int consonants = 0;
    private int digits = 0;
    private int aaBatteries = 0;
    private int ports = 0;
    private int modules = 0;
    private int indicators = 0;
    private int portPlates = 0;
    private int listed = -1;

    //Correct button list
    public List<KMSelectable> correctButtons = new List<KMSelectable>();
    public List<KMSelectable> correctButtonsOrdered = new List<KMSelectable>();
    private List<Light> correctLights = new List<Light>();
    private List<Light> correctLightsOrdered = new List<Light>();
    public List<int> serialNumberEntries = new List<int>();
    public List<int> serialNumberEntriesWarped = new List<int>();
    private int orderOfRotation = 0;

    //Logging etc.
    static int moduleIdCounter = 1;
    int moduleId;
    private int stage = 1;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable button in buttons)
        {
            KMSelectable trueButton = button;
            button.OnInteract += delegate () { Onbutton(trueButton); return false; };
        }
    }

    void Start()
    {
        foreach (Light x in lights)
        {
            x.enabled = false;
        }
        LightPicker();
        serialDigitsTotal = Bomb.GetSerialNumberNumbers().Sum();
        Debug.LogFormat("[The Moon #{0}] Count {1} on unlit sets from {2} position.", moduleId, serialDigitsTotal, offLightsName[3]);
        startPosition = offLights[(3 + serialDigitsTotal) % 4];
        Debug.LogFormat("[The Moon #{0}] Your start position is {1}.", moduleId, positions[startPosition - 1]);
        SectionDetermination();
        SelectionOrder();
    }

    void LightPicker()
    {
        if (lightsPicked == false)
        {
            lightIndex = UnityEngine.Random.Range(0,8);
            lightsPicked = true;
        }
        lights[lightIndex].enabled = true;
        lights[lightIndex + 8].enabled = true;
        lights[(lightIndex + 1) % 8].enabled = true;
        lights[(lightIndex + 1) % 8 + 8].enabled = true;
        lights[(lightIndex + 2) % 8].enabled = true;
        lights[(lightIndex + 2) % 8 + 8].enabled = true;
        lights[(lightIndex + 3) % 8].enabled = true;
        lights[(lightIndex + 3) % 8 + 8].enabled = true;
        offLights.Add((lightIndex + 4) % 8 + 1);
        offLightsName.Add(positions[(lightIndex + 4) % 8]);
        offLights.Add((lightIndex + 5) % 8 + 1);
        offLightsName.Add(positions[(lightIndex + 5) % 8]);
        offLights.Add((lightIndex + 6) % 8 + 1);
        offLightsName.Add(positions[(lightIndex + 6) % 8]);
        offLights.Add((lightIndex + 7) % 8 + 1);
        offLightsName.Add(positions[(lightIndex + 7) % 8]);
        Debug.LogFormat("[The Moon #{0}] Lit sets are: {1}, {2}, {3} & {4}.", moduleId, positions[lightIndex], positions[(lightIndex + 1) % 8], positions[(lightIndex + 2) % 8], positions[(lightIndex + 3) % 8]);
    }

    void StartLights()
    {
        foreach (Light x in lights)
        {
            x.enabled = false;
        }
        lights[lightIndex].enabled = true;
        lights[lightIndex + 8].enabled = true;
        lights[(lightIndex + 1) % 8].enabled = true;
        lights[(lightIndex + 1) % 8 + 8].enabled = true;
        lights[(lightIndex + 2) % 8].enabled = true;
        lights[(lightIndex + 2) % 8 + 8].enabled = true;
        lights[(lightIndex + 3) % 8].enabled = true;
        lights[(lightIndex + 3) % 8 + 8].enabled = true;
    }

    void SectionDetermination()
    {
        dBatteries = Bomb.GetBatteryCount(Battery.D) % 7;
        consonants = Bomb.GetSerialNumberLetters().Count(x => x != 'A' && x != 'E' && x != 'I' && x != 'O' && x != 'U') % 7;
        digits = Bomb.GetSerialNumberNumbers().Count() % 7;
        aaBatteries = Bomb.GetBatteryCount(Battery.AA) + Bomb.GetBatteryCount(Battery.AAx3) + Bomb.GetBatteryCount(Battery.AAx4) % 7;
        ports = Bomb.GetPortCount() % 7;
        modules = Bomb.GetModuleNames().Count() % 7;
        indicators = Bomb.GetIndicators().Count() % 7;
        portPlates = Bomb.GetPortPlates().Count() % 7;
        Debug.LogFormat("[The Moon #{0}] # of D batteries = {1}.", moduleId, dBatteries);
        Debug.LogFormat("[The Moon #{0}] # of consonants = {1}.", moduleId, consonants);
        Debug.LogFormat("[The Moon #{0}] # of serial digits = {1}.", moduleId, digits);
        Debug.LogFormat("[The Moon #{0}] # of AA batteries = {1}.", moduleId, aaBatteries);
        Debug.LogFormat("[The Moon #{0}] # of ports = {1}.", moduleId, ports);
        Debug.LogFormat("[The Moon #{0}] # of modules = {1}.", moduleId, modules);
        Debug.LogFormat("[The Moon #{0}] # of indicators = {1}.", moduleId, indicators);
        Debug.LogFormat("[The Moon #{0}] # of port plates = {1}.", moduleId, portPlates);

        if (startPosition == offLights[0])
        {
            modulesMethod();
        }
        else if (startPosition == offLights[1])
        {
            consonantsMethod();
        }
        else if (startPosition == offLights[2])
        {
            indicatorsMethod();
        }
        else if (startPosition == offLights[3])
        {
            portsMethod();
        }
    }

    void modulesMethod()
    {
        if (modules < 3)
        {
            correctButtons.Add(buttons[(startPosition + listed) % 8]);
            correctLights.Add(lights[(startPosition + listed) % 8]);
        }
        else if (modules >= 3 && modules < 5)
        {
            correctButtons.Add(buttons[((startPosition + listed) % 8) + 8]);
            correctLights.Add(lights[((startPosition + listed) % 8) + 8]);
        }
        else
        {
            correctButtons.Add(buttons[16]);
            correctLights.Add(lights[16]);
        }
        if (correctButtons.Count() == 8)
        {
            return;
        }
        else
        {
            listed++;
            consonantsMethod();
        }
    }

    void consonantsMethod()
    {
        if (consonants < 3)
        {
            correctButtons.Add(buttons[(startPosition + listed) % 8]);
            correctLights.Add(lights[(startPosition + listed) % 8]);
        }
        else if (consonants >= 3 && consonants < 5)
        {
            correctButtons.Add(buttons[((startPosition + listed) % 8) + 8]);
            correctLights.Add(lights[((startPosition + listed) % 8) + 8]);
        }
        else
        {
            correctButtons.Add(buttons[16]);
            correctLights.Add(lights[16]);
        }
        if (correctButtons.Count() == 8)
        {
            return;
        }
        else
        {
            listed++;
            indicatorsMethod();
        }
    }

    void indicatorsMethod()
    {
        if (indicators < 3)
        {
            correctButtons.Add(buttons[(startPosition + listed) % 8]);
            correctLights.Add(lights[(startPosition + listed) % 8]);
        }
        else if (indicators >= 3 && indicators < 5)
        {
            correctButtons.Add(buttons[((startPosition + listed) % 8) + 8]);
            correctLights.Add(lights[((startPosition + listed) % 8) + 8]);
        }
        else
        {
            correctButtons.Add(buttons[16]);
            correctLights.Add(lights[16]);
        }
        if (correctButtons.Count() == 8)
        {
            return;
        }
        else
        {
            listed++;
            portsMethod();
        }
    }

    void portsMethod()
    {
        if (ports < 3)
        {
            correctButtons.Add(buttons[(startPosition + listed) % 8]);
            correctLights.Add(lights[(startPosition + listed) % 8]);
        }
        else if (ports >= 3 && ports < 5)
        {
            correctButtons.Add(buttons[((startPosition + listed) % 8) + 8]);
            correctLights.Add(lights[((startPosition + listed) % 8) + 8]);
        }
        else
        {
            correctButtons.Add(buttons[16]);
            correctLights.Add(lights[16]);
        }
        if (correctButtons.Count() == 8)
        {
            return;
        }
        else
        {
            listed++;
            dBattMethod();
        }
    }

    void dBattMethod()
    {
        if (dBatteries < 3)
        {
            correctButtons.Add(buttons[(startPosition + listed) % 8]);
            correctLights.Add(lights[(startPosition + listed) % 8]);
        }
        else if (dBatteries >= 3 && dBatteries < 5)
        {
            correctButtons.Add(buttons[((startPosition + listed) % 8) + 8]);
            correctLights.Add(lights[((startPosition + listed) % 8) + 8]);
        }
        else
        {
            correctButtons.Add(buttons[16]);
            correctLights.Add(lights[16]);
        }
        if (correctButtons.Count() == 8)
        {
            return;
        }
        else
        {
            listed++;
            aaBattMethod();
        }
    }

    void aaBattMethod()
    {
        if (aaBatteries < 3)
        {
            correctButtons.Add(buttons[(startPosition + listed) % 8]);
            correctLights.Add(lights[(startPosition + listed) % 8]);
        }
        else if (aaBatteries >= 3 && aaBatteries < 5)
        {
            correctButtons.Add(buttons[((startPosition + listed) % 8) + 8]);
            correctLights.Add(lights[((startPosition + listed) % 8) + 8]);
        }
        else
        {
            correctButtons.Add(buttons[16]);
            correctLights.Add(lights[16]);
        }
        if (correctButtons.Count() == 8)
        {
            return;
        }
        else
        {
            listed++;
            digitsMethod();
        }
    }

    void digitsMethod()
    {
        if (digits < 3)
        {
            correctButtons.Add(buttons[(startPosition + listed) % 8]);
            correctLights.Add(lights[(startPosition + listed) % 8]);
        }
        else if (digits >= 3 && digits < 5)
        {
            correctButtons.Add(buttons[((startPosition + listed) % 8) + 8]);
            correctLights.Add(lights[((startPosition + listed) % 8) + 8]);
        }
        else
        {
            correctButtons.Add(buttons[16]);
            correctLights.Add(lights[16]);
        }
        if (correctButtons.Count() == 8)
        {
            return;
        }
        else
        {
            listed++;
            portPlatesMethod();
        }
    }

    void portPlatesMethod()
    {
        if (portPlates < 3)
        {
            correctButtons.Add(buttons[(startPosition + listed) % 8]);
            correctLights.Add(lights[(startPosition + listed) % 8]);
        }
        else if (portPlates >= 3 && portPlates < 5)
        {
            correctButtons.Add(buttons[((startPosition + listed) % 8) + 8]);
            correctLights.Add(lights[((startPosition + listed) % 8) + 8]);
        }
        else
        {
            correctButtons.Add(buttons[16]);
            correctLights.Add(lights[16]);
        }
        if (correctButtons.Count() == 8)
        {
            return;
        }
        else
        {
            listed++;
            modulesMethod();
        }
    }

    void SelectionOrder()
    {
        serialNumberEntries = (Bomb.GetSerialNumber().Select(c => Char.IsDigit(c) ? c - '0' : c - '@')).ToList();
        for (int serialEntry = 0; serialEntry < serialNumberEntries.Count; ++serialEntry)
        {
            int digit = serialNumberEntries[serialEntry];
            if (digit < 10 && digit >= 1)
            {
                digit *= 10;
            }
            else if (digit == 0)
            {
                digit += 10;
            }
            else
            {
                int secondDigit = digit % 10;
                int secondDigitTimes = secondDigit * 10;
                int firstDigit = (digit - secondDigit) / 10;
                digit = secondDigitTimes + firstDigit;
            }
            serialNumberEntriesWarped.Add(digit % 7);
        }
        listed = -1;
        correctButtonsOrdered.Add(correctButtons[0]);
        correctLightsOrdered.Add(correctLights[0]);
        listed++;
        OrderingLogic();
    }

    void OrderingLogic()
    {
        foreach (int digit in serialNumberEntriesWarped)
        {
            if (digit < 4)
            {
                orderOfRotation = (orderOfRotation + 2) % 8;
                while (correctButtonsOrdered.Contains(correctButtons[orderOfRotation]))
                {
                    orderOfRotation = (orderOfRotation + 1) % 8;
                }
                correctButtonsOrdered.Add(correctButtons[orderOfRotation]);
                correctLightsOrdered.Add(correctLights[orderOfRotation]);
            }
            else if (digit > 3)
            {
                orderOfRotation = (orderOfRotation + 6) % 8;
                while (correctButtonsOrdered.Contains(correctButtons[orderOfRotation]))
                {
                    orderOfRotation = (orderOfRotation + 7) % 8;
                }
                correctButtonsOrdered.Add(correctButtons[orderOfRotation]);
                correctLightsOrdered.Add(correctLights[orderOfRotation]);
            }
        }
        correctButtonsOrdered.AddRange(correctButtons.Except(correctButtonsOrdered));
        correctLightsOrdered.AddRange(correctLights.Except(correctLightsOrdered));

        Debug.LogFormat("[The Moon #{0}] The first button is {1}.", moduleId, correctButtonsOrdered[0].name);
        if (correctButtonsOrdered[0] != buttons[16])
        {
            Debug.LogFormat("[The Moon #{0}] The second button is {1}.", moduleId, correctButtonsOrdered[1].name);
            if (correctButtonsOrdered[1] != buttons[16])
            {
                Debug.LogFormat("[The Moon #{0}] The third button is {1}.", moduleId, correctButtonsOrdered[2].name);
                if (correctButtonsOrdered[2] != buttons[16])
                {
                    Debug.LogFormat("[The Moon #{0}] The fourth button is {1}.", moduleId, correctButtonsOrdered[3].name);
                    if (correctButtonsOrdered[3] != buttons[16])
                    {
                        Debug.LogFormat("[The Moon #{0}] The fifth button is {1}.", moduleId, correctButtonsOrdered[4].name);
                        if (correctButtonsOrdered[4] != buttons[16])
                        {
                            Debug.LogFormat("[The Moon #{0}] The sixth button is {1}.", moduleId, correctButtonsOrdered[5].name);
                            if (correctButtonsOrdered[5] != buttons[16])
                            {
                                Debug.LogFormat("[The Moon #{0}] The seventh button is {1}.", moduleId, correctButtonsOrdered[6].name);
                                if (correctButtonsOrdered[6] != buttons[16])
                                {
                                    Debug.LogFormat("[The Moon #{0}] The eighth button is {1}.", moduleId, correctButtonsOrdered[7].name);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void Onbutton(KMSelectable button)
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        GetComponent<KMSelectable>().AddInteractionPunch();

        switch (stage)
        {
            case 1:
            if (button == correctButtonsOrdered[0])
            {
                Debug.LogFormat("[The Moon #{0}] You pressed {1}. That is correct.", moduleId, correctButtonsOrdered[0].name);
                if (correctButtonsOrdered[0] != buttons[16])
                {
                    if (correctLightsOrdered[0].enabled == true)
                    {
                        correctLightsOrdered[0].enabled = false;
                    }
                    else
                    {
                        correctLightsOrdered[0].enabled = true;
                    }
                    Audio.PlaySoundAtTransform("tone1", transform);
                    stage++;
                }
                else
                {
                    lights[16].enabled = true;
                    Audio.PlaySoundAtTransform("tone8", transform);
                    ModuleSolved();
                }
            }
            else
            {
                Debug.LogFormat("[The Moon #{0}] Strike! You pressed {1}. That is incorrect. I was expecting {2}.", moduleId, button.name, correctButtonsOrdered[0].name);
                stage = 1;
                StartLights();
                GetComponent<KMBombModule>().HandleStrike();
            }
            break;

            case 2:
            if (button == correctButtonsOrdered[1])
            {
                Debug.LogFormat("[The Moon #{0}] You pressed {1}. That is correct.", moduleId, correctButtonsOrdered[1].name);
                if (correctButtonsOrdered[1] != buttons[16])
                {
                    if (correctLightsOrdered[1].enabled == true)
                    {
                        correctLightsOrdered[1].enabled = false;
                    }
                    else
                    {
                        correctLightsOrdered[1].enabled = true;
                    }
                    Audio.PlaySoundAtTransform("tone2", transform);
                    stage++;
                }
                else
                {
                    lights[16].enabled = true;
                    Audio.PlaySoundAtTransform("tone8", transform);
                    ModuleSolved();
                }
            }
            else
            {
                Debug.LogFormat("[The Moon #{0}] Strike! You pressed {1}. That is incorrect. I was expecting {2}.", moduleId, button.name, correctButtonsOrdered[1].name);
                stage = 1;
                StartLights();
                GetComponent<KMBombModule>().HandleStrike();
            }
            break;

            case 3:
            if (button == correctButtonsOrdered[2])
            {
                Debug.LogFormat("[The Moon #{0}] You pressed {1}. That is correct.", moduleId, correctButtonsOrdered[2].name);
                if (correctButtonsOrdered[2] != buttons[16])
                {
                    if (correctLightsOrdered[2].enabled == true)
                    {
                        correctLightsOrdered[2].enabled = false;
                    }
                    else
                    {
                        correctLightsOrdered[2].enabled = true;
                    }
                    Audio.PlaySoundAtTransform("tone3", transform);
                    stage++;
                }
                else
                {
                    lights[16].enabled = true;
                    Audio.PlaySoundAtTransform("tone8", transform);
                    ModuleSolved();
                }
            }
            else
            {
                Debug.LogFormat("[The Moon #{0}] Strike! You pressed {1}. That is incorrect. I was expecting {2}.", moduleId, button.name, correctButtonsOrdered[2].name);
                stage = 1;
                StartLights();
                GetComponent<KMBombModule>().HandleStrike();
            }
            break;

            case 4:
            if (button == correctButtonsOrdered[3])
            {
                Debug.LogFormat("[The Moon #{0}] You pressed {1}. That is correct.", moduleId, correctButtonsOrdered[3].name);
                if (correctButtonsOrdered[3] != buttons[16])
                {
                    if (correctLightsOrdered[3].enabled == true)
                    {
                        correctLightsOrdered[3].enabled = false;
                    }
                    else
                    {
                        correctLightsOrdered[3].enabled = true;
                    }
                    Audio.PlaySoundAtTransform("tone4", transform);
                    stage++;
                }
                else
                {
                    lights[16].enabled = true;
                    Audio.PlaySoundAtTransform("tone8", transform);
                    ModuleSolved();
                }
            }
            else
            {
                Debug.LogFormat("[The Moon #{0}] Strike! You pressed {1}. That is incorrect. I was expecting {2}.", moduleId, button.name, correctButtonsOrdered[3].name);
                stage = 1;
                StartLights();
                GetComponent<KMBombModule>().HandleStrike();
            }
            break;

            case 5:
            if (button == correctButtonsOrdered[4])
            {
                Debug.LogFormat("[The Moon #{0}] You pressed {1}. That is correct.", moduleId, correctButtonsOrdered[4].name);
                if (correctButtonsOrdered[4] != buttons[16])
                {
                    if (correctLightsOrdered[4].enabled == true)
                    {
                        correctLightsOrdered[4].enabled = false;
                    }
                    else
                    {
                        correctLightsOrdered[4].enabled = true;
                    }
                    Audio.PlaySoundAtTransform("tone5", transform);
                    stage++;
                }
                else
                {
                    lights[16].enabled = true;
                    Audio.PlaySoundAtTransform("tone8", transform);
                    ModuleSolved();
                }
            }
            else
            {
                Debug.LogFormat("[The Moon #{0}] Strike! You pressed {1}. That is incorrect. I was expecting {2}.", moduleId, button.name, correctButtonsOrdered[4].name);
                stage = 1;
                StartLights();
                GetComponent<KMBombModule>().HandleStrike();
            }
            break;

            case 6:
            if (button == correctButtonsOrdered[5])
            {
                Debug.LogFormat("[The Moon #{0}] You pressed {1}. That is correct.", moduleId, correctButtonsOrdered[5].name);
                if (correctButtonsOrdered[5] != buttons[16])
                {
                    if (correctLightsOrdered[5].enabled == true)
                    {
                        correctLightsOrdered[5].enabled = false;
                    }
                    else
                    {
                        correctLightsOrdered[5].enabled = true;
                    }
                    Audio.PlaySoundAtTransform("tone6", transform);
                    stage++;
                }
                else
                {
                    lights[16].enabled = true;
                    Audio.PlaySoundAtTransform("tone8", transform);
                    ModuleSolved();
                }
            }
            else
            {
                Debug.LogFormat("[The Moon #{0}] Strike! You pressed {1}. That is incorrect. I was expecting {2}.", moduleId, button.name, correctButtonsOrdered[5].name);
                stage = 1;
                StartLights();
                GetComponent<KMBombModule>().HandleStrike();
            }
            break;

            case 7:
            if (button == correctButtonsOrdered[6])
            {
                Debug.LogFormat("[The Moon #{0}] You pressed {1}. That is correct.", moduleId, correctButtonsOrdered[6].name);
                if (correctButtonsOrdered[6] != buttons[16])
                {
                    if (correctLightsOrdered[6].enabled == true)
                    {
                        correctLightsOrdered[6].enabled = false;
                    }
                    else
                    {
                        correctLightsOrdered[6].enabled = true;
                    }
                    Audio.PlaySoundAtTransform("tone7", transform);
                    stage++;
                }
                else
                {
                    lights[16].enabled = true;
                    Audio.PlaySoundAtTransform("tone8", transform);
                    ModuleSolved();
                }
            }
            else
            {
                Debug.LogFormat("[The Moon #{0}] Strike! You pressed {1}. That is incorrect. I was expecting {2}.", moduleId, button.name, correctButtonsOrdered[6].name);
                stage = 1;
                StartLights();
                GetComponent<KMBombModule>().HandleStrike();
            }
            break;

            case 8:
            if (button == correctButtonsOrdered[7])
            {
                Debug.LogFormat("[The Moon #{0}] You pressed {1}. That is correct.", moduleId, correctButtonsOrdered[7].name);
                if (correctButtonsOrdered[7] != buttons[16])
                {
                    if (correctLightsOrdered[7].enabled == true)
                    {
                        correctLightsOrdered[7].enabled = false;
                    }
                    else
                    {
                        correctLightsOrdered[7].enabled = true;
                    }
                    Audio.PlaySoundAtTransform("tone8", transform);
                    stage++;
                    ModuleSolved();
                }
                else
                {
                    lights[16].enabled = true;
                    Audio.PlaySoundAtTransform("tone8", transform);
                    ModuleSolved();
                }
            }
            else
            {
                Debug.LogFormat("[The Moon #{0}] Strike! You pressed {1}. That is incorrect. I was expecting {2}.", moduleId, button.name, correctButtonsOrdered[7].name);
                stage = 1;
                StartLights();
                GetComponent<KMBombModule>().HandleStrike();
            }
            break;

            default:
            break;
        }
    }

    void ModuleSolved()
    {
        stage = 9;
        StartCoroutine(lightDance());
        Debug.LogFormat("[The Moon #{0}] Module disarmed.", moduleId);
        GetComponent<KMBombModule>().HandlePass();
    }

    private IEnumerator lightDance()
    {
        yield return new WaitForSeconds(0.3f);

        if (lightCount == 0)
        {
            Audio.PlaySoundAtTransform("toneFinish", transform);
        }

        while (lightCount < 4)
        {
            yield return new WaitForSeconds(0.25f);
            foreach (Light light in correctLightsOrdered)
            {
                light.enabled = false;
            }
            lights[0].enabled = true;
            lights[2].enabled = true;
            lights[4].enabled = true;
            lights[6].enabled = true;
            lights[8].enabled = true;
            lights[10].enabled = true;
            lights[12].enabled = true;
            lights[14].enabled = true;
            lights[1].enabled = false;
            lights[3].enabled = false;
            lights[5].enabled = false;
            lights[7].enabled = false;
            lights[9].enabled = false;
            lights[11].enabled = false;
            lights[13].enabled = false;
            lights[15].enabled = false;
            lights[16].enabled = false;

            yield return new WaitForSeconds(0.25f);
            lights[0].enabled = false;
            lights[2].enabled = false;
            lights[4].enabled = false;
            lights[6].enabled = false;
            lights[8].enabled = false;
            lights[10].enabled = false;
            lights[12].enabled = false;
            lights[14].enabled = false;
            lights[1].enabled = true;
            lights[3].enabled = true;
            lights[5].enabled = true;
            lights[7].enabled = true;
            lights[9].enabled = true;
            lights[11].enabled = true;
            lights[13].enabled = true;
            lights[15].enabled = true;
            lights[16].enabled = true;
            lightCount++;
        }
        lights[0].enabled = true;
        lights[2].enabled = true;
        lights[4].enabled = true;
        lights[6].enabled = true;
        lights[8].enabled = true;
        lights[10].enabled = true;
        lights[12].enabled = true;
        lights[14].enabled = true;
        lights[1].enabled = true;
        lights[3].enabled = true;
        lights[5].enabled = true;
        lights[7].enabled = true;
        lights[9].enabled = true;
        lights[11].enabled = true;
        lights[13].enabled = true;
        lights[15].enabled = true;
        lights[16].enabled = true;
    }
    
    #pragma warning disable 414
    private string TwitchHelpMessage = @"Use “!{0} press inner top” to press the inner top button. Use “!{0} press outer bottomleft” to press the outer top left button. Use “!{0} press center” to press the center button. Combine the commands using a colon (;). NEWS directions (North-East-South-West) and shortened directions (“t” and “n”) also work.";
    #pragma warning restore 414

    private static string[] supportedSections = new[] { "inner", "outer" };
    private static string[] supportedDirections = new[] { "top", "bottom", "left", "right", "topleft", "topright", "bottomleft", "bottomright", "north", "south", "east", "west", "northwest", "northeast", "southwest", "southeast", "n", "s", "e", "w", "nw", "ne", "sw", "se", "t", "b", "l", "r", "tl", "bl", "tr", "br" };

    IEnumerator ProcessTwitchCommand(string command)
    {
        var parts = command.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length > 1 && parts[0] == "press")
        {
            var cmdButtons = command.ToLowerInvariant().Replace("press ", "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            bool goodCommands = true;

            foreach (string cmd in cmdButtons)
            {
                if (!((cmd.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length == 1 && cmd.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0] == "center") || (cmd.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length == 2 && supportedSections.Contains(cmd.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]) && supportedDirections.Contains(cmd.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]))))
                {
                    goodCommands = false;
                }
            }

            if (goodCommands)
            {
                yield return null;

                foreach (string cmd in cmdButtons)
                {
                    var split = cmd.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length == 1)
                    {
                        CheckAndPress("center", "");
                    }
                    else
                    {
                        CheckAndPress(split[0], split[1]);
                    }

                    yield return new WaitForSeconds(.2f);
                }
                yield break;
            }

        }
    }

    void CheckAndPress(string section, string direction)
    {
        if (new[] { "top", "t", "north", "n" }.Contains(direction))
        {
            if (section == "outer")
            {
                Onbutton(buttons[0]);
            }
            else
            {
                Onbutton(buttons[8]);
            }
        }
        else if (new[] { "topright", "tr", "northeast", "ne" }.Contains(direction))
        {
            if (section == "outer")
            {
                Onbutton(buttons[1]);
            }
            else
            {
                Onbutton(buttons[9]);
            }
        }
        else if (new[] { "right", "r", "east", "e" }.Contains(direction))
        {
            if (section == "outer")
            {
                Onbutton(buttons[2]);
            }
            else
            {
                Onbutton(buttons[10]);
            }
        }
        else if (new[] { "bottomright", "br", "southeast", "se" }.Contains(direction))
        {
            if (section == "outer")
            {
                Onbutton(buttons[3]);
            }
            else
            {
                Onbutton(buttons[11]);
            }
        }
        else if (new[] { "bottom", "b", "south", "s" }.Contains(direction))
        {
            if (section == "outer")
            {
                Onbutton(buttons[4]);
            }
            else
            {
                Onbutton(buttons[12]);
            }
        }
        else if (new[] { "bottomleft", "bl", "southwest", "sw" }.Contains(direction))
        {
            if (section == "outer")
            {
                Onbutton(buttons[5]);
            }
            else
            {
                Onbutton(buttons[13]);
            }
        }
        else if (new[] { "left", "l", "west", "w" }.Contains(direction))
        {
            if (section == "outer")
            {
                Onbutton(buttons[6]);
            }
            else
            {
                Onbutton(buttons[14]);
            }
        }
        else if (new[] { "topleft", "tl", "northwest", "nw" }.Contains(direction))
        {
            if (section == "outer")
            {
                Onbutton(buttons[7]);
            }
            else
            {
                Onbutton(buttons[15]);
            }
        }
        else if (section == "center")
        {
            Onbutton(buttons[16]);
        }
    }
}
}
