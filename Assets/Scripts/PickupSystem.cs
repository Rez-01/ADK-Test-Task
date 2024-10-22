using TMPro;
using UnityEngine;

public class PickupSystem : MonoBehaviour
{
    [SerializeField] private float _pickupRange;
    [SerializeField] private Transform _holdPosition;
    [SerializeField] private Material _highlightMaterial;
    [SerializeField] private TMP_Text _notificationText;
    
    private Material _originalMaterial;
    private Rigidbody _itemRigidbody;
    private bool _isHoldingItem;
    private Renderer _itemRenderer;

    private GameObject _currentItem;
    private GameObject _highlightedItem;

    private void Update()
    {
        if (_isHoldingItem)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                DropItem();
            }
        }
        else
        {
            HighlightItem();

            if (Input.GetKeyDown(KeyCode.E))
            {
                TryPickupItem();
            }
        }
    }

    private void HighlightItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _pickupRange))
        {
            if (hit.collider.TryGetComponent(out PickupItem pickupItem))
            {
                GameObject hitItem = pickupItem.gameObject;

                if (_highlightedItem != hitItem)
                {
                    if (_highlightedItem != null)
                    {
                        RemoveHighlight();
                    }

                    _highlightedItem = hitItem;
                    _itemRenderer = _highlightedItem.GetComponent<Renderer>();
                    _notificationText.text = "Press E to grab";

                    if (_itemRenderer != null)
                    {
                        _originalMaterial = _itemRenderer.material;
                        Debug.Log(_originalMaterial);
                        _itemRenderer.material = _highlightMaterial;
                    }
                }
            }
            else
            {
                RemoveHighlight();
            }
        }
        else
        {
            RemoveHighlight();
        }
    }

    private void RemoveHighlight()
    {
        RestoreOriginalMaterial();
        _notificationText.text = "";
        _highlightedItem = null;
    }
    
    private void RestoreOriginalMaterial()
    {
        if (_itemRenderer != null && _originalMaterial != null)
        {
            Debug.Log("Original material is " + _originalMaterial);
            _itemRenderer.material = _originalMaterial;
        }
    }

    private void TryPickupItem()
    {
        if (_highlightedItem != null)
        {
            _currentItem = _highlightedItem;
            _itemRigidbody = _currentItem.GetComponentInChildren<Rigidbody>();

            if (_itemRigidbody != null)
            {
                PickupItem();
            }
        }
    }

    private void PickupItem()
    {
        _isHoldingItem = true;
        _itemRigidbody.useGravity = false;
        _itemRigidbody.isKinematic = true;
        
        _currentItem.gameObject.transform.SetParent(_holdPosition);
        _currentItem.gameObject.transform.localPosition = Vector3.zero;
        _currentItem.gameObject.transform.localRotation = Quaternion.identity;
        
        _notificationText.text = "Press E to drop item";

        RestoreOriginalMaterial();
        _highlightedItem = null;
    }

    private void DropItem()
    {
        if (_currentItem == null) return;

        _isHoldingItem = false;
        
        _currentItem.gameObject.transform.SetParent(null);

        _itemRigidbody.useGravity = true;
        _itemRigidbody.isKinematic = false;
        
        _itemRigidbody.AddForce(Camera.main.transform.forward * 2f, ForceMode.Impulse);

        _currentItem = null;
        
        _notificationText.text = "";
    }
}