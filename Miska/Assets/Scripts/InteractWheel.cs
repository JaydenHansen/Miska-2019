using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A wheel that allows the selection of different actions using the Quicktime(interaction) system
/// </summary>
public class InteractWheel : MonoBehaviour
{
    public Image[] m_images;
    public Image m_mousePos;
    public float m_mouseSpeed;
    public float m_maxMagnitude = 40;
    public float m_deadZone = 10;
    public float m_unselectedAlpha;

    float m_angle;
    Vector2 m_mousePosition;
    int m_currentSelected;
    bool m_enabled;
    bool[] m_disabledOptions;

    // Start is called before the first frame update
    void Start()
    {
        m_angle = 360f / m_images.Length;
        m_mousePosition = Vector2.zero;
        m_disabledOptions = new bool[m_images.Length];

        foreach(Image image in m_images)
        {
            Color newColor = image.color;
            newColor.a = m_unselectedAlpha;
            image.color = newColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // moves the cursor around the avaliable area
        m_mousePosition += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * m_mouseSpeed;
        m_mousePosition = Vector2.ClampMagnitude(m_mousePosition, m_maxMagnitude);
        m_mousePos.rectTransform.localPosition = m_mousePosition;

        if (m_mousePosition.magnitude > m_deadZone) // if the cursor is out of the centre deadzone
        {
            float angleBetween = -Vector2.SignedAngle(Vector2.up, m_mousePosition); // gets the angle of the cursor's position
            if (angleBetween < 0) // keeps the angel between [0, 360]
                angleBetween += 360;

            for (int i = 0; i < m_images.Length; i++) // foreach image
            {
                if (i != 0) // if the current image is not the first one
                {
                    if (angleBetween > (i * m_angle) - (m_angle * 0.5f) && angleBetween <= (i * m_angle) + (m_angle * 0.5f)) // if the angle between is in the current images section
                    {
                        ChangeSelected(i); // change the current selected to the index of the current image
                    }
                }
                else
                {
                    if ((angleBetween <= (m_angle * 0.5f)) || (angleBetween > 360 - (m_angle * 0.5f))) // if the angle between is in the first images section
                    {
                        ChangeSelected(i); // change the current selected to the index of the current image
                    }
                }
            }
        }
        else
        {
            ChangeSelected(-1); // none are selected
        }
    }

    /// <summary>
    /// Changes the currently selected image
    /// </summary>
    /// <param name="newSelected">the index of the new image</param>
    void ChangeSelected(int newSelected)
    {
        if (m_currentSelected == newSelected) // can't change to the same image
            return;

        if (m_currentSelected >= 0) // if there is currently a selected image
        {
            // revert the currently selected image's alpha back to the unselected alpha
            Color newColor = m_images[m_currentSelected].color;
            newColor.a = m_unselectedAlpha;
            m_images[m_currentSelected].color = newColor;
        }

        m_currentSelected = newSelected; // change the current selection to the new one

        if (m_currentSelected >= 0 && !m_disabledOptions[m_currentSelected]) // if the new selection is an actual selection (not -1)
        {
            // change the newly selected image's alpha to full
            Color newColor = m_images[m_currentSelected].color;
            newColor.a = 1;
            m_images[m_currentSelected].color = newColor;
        }
    }

    /// <summary>
    /// Enables the interact wheel and resets the values
    /// </summary>
    public void EnableWheel()
    {
        gameObject.SetActive(true);
        ChangeSelected(-1);
        m_mousePosition = Vector2.zero;
        m_mousePos.rectTransform.localPosition = m_mousePosition;
        m_enabled = true;
    }

    /// <summary>
    /// Disables the interact wheel and activates the selection
    /// </summary>
    public void DisableWheel()
    {
        if (m_enabled) // only disable when actually enabled
        {
            if (m_currentSelected >= 0 && !m_disabledOptions[m_currentSelected]) // if there is a current selection thats not disabled
            {
                QuicktimeResponse[] responses = m_images[m_currentSelected].GetComponents<QuicktimeResponse>(); // get the array of quicktime responses on the image's object
                foreach (QuicktimeResponse response in responses) // call the on success function for each response
                {
                    response.OnSuccess(); 
                }
            }

            // disable the interact wheel
            gameObject.SetActive(false);
            m_enabled = false;
        }
    }

    /// <summary>
    /// Disables an option on the wheel
    /// </summary>
    /// <param name="index">The index of the option to disable</param>
    public void DisableOption(int index)
    {
        if (index >= 0 && index < m_disabledOptions.Length) // if the index is within the bounds
        {
            m_disabledOptions[index] = true; // sets the image at index to disabled
        }
        else
        {
            Debug.Log("Disabling Option out of bounds");
        }
    }

    /// <summary>
    /// Enables and option on the wheel
    /// </summary>
    /// <param name="index">the index of the option to enable</param>
    public void EnableOption(int index)
    {
        if (index >= 0 && index < m_disabledOptions.Length) // if the index is within the bounds
        {
            m_disabledOptions[index] = false; // sets the image at index to enabled
        }
        else
        {
            Debug.Log("Enabling Option out of bounds");
        }
    }
}
