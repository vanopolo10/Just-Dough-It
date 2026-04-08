using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShopManager : MonoBehaviour
{

    [SerializeField] private List<ShopBook> _books;
    [SerializeField] private float _displayDelay = 0.5f;
    [SerializeField] private AnimationClip _movementClip;
    [SerializeField] private float _clipSpeed = 2, _delayAfterClipFinish = 0f, _stopDistance = 0.1f;
    
    //[SerializeField] private AnimationCurve _YposCurve;
    private bool _inPosition = false;
    [SerializeField] private int _currentlyMovingBooks = 0;
    private int _currentBook = 0;

    public int CurrentBook => _currentBook;
    public List<ShopBook> Books => _books;

    private void OnMouseDown()
    {
        if (_currentlyMovingBooks != 0) return;
        if (_currentBook >= _books.Count)
        {
            Debug.Log("[ShopManager] No more books to display");
            return;
        }

        Debug.Log($"[ShopManager] Beginning to display current book {_currentBook}, inPosition is {_inPosition}");

        //StartCoroutine(MoveAllBooks());
        StartCoroutine(MoveBookRoutine(_books[_currentBook], _inPosition));
        _inPosition = !_inPosition;
    }
    public void CycleBook() {
        Debug.Log($"[ShopManager] Beginning to cycle book {_currentBook}");
        _currentBook++;
        if (_currentBook >= _books.Count) { 
            Debug.Log("[ShopManager] No more books to display");
            return;
        }

        
        StartCoroutine(MoveBookRoutine(_books[_currentBook], false));
        _inPosition = true;
    }
    private IEnumerator MoveAllBooks() {
        bool toInitial = _inPosition;
        _inPosition = !_inPosition;

        _books.Reverse();
        foreach (var book in _books)
        {
            StartCoroutine(MoveBookRoutine(book, toInitial));
            yield return new WaitForSeconds(_displayDelay);
        }

        yield break;
    }
    private IEnumerator MoveBookRoutine(ShopBook book, bool toInitial = false) {
        Debug.Log($"[ShopManager] moving book with init pos {book.InitialPosition}, toInitial is {toInitial}");
        _currentlyMovingBooks++;
        if( toInitial ) book.OnMovedOutOfPosition();

        Vector3 target = book.TargetPosition;
        if (toInitial)
        {
            target = book.InitialPosition;
        }
        Vector3 init = book.Book.transform.position;

        book.Animator?.SetTrigger("StartFlying");

        float time = (_movementClip.length / _clipSpeed) + _delayAfterClipFinish;
        float distance = Vector3.Distance(init, target);
        float speed = distance / time;
        Debug.Log($"[ShopManager] BookMover calculated variables: {time} {distance} {speed}");

        while (Vector3.Distance(book.Book.transform.position, target) > _stopDistance) {
            Vector3 pos = book.Book.transform.position;

            pos = Vector3.MoveTowards(book.Book.transform.position, target, speed * Time.deltaTime);

            /*
            float progress = (pos.x - init.x) / (target.x - init.x);
            pos.y = pos.y + _YposCurve.Evaluate(progress);
            Debug.Log($"book move evaluated Y to {_YposCurve.Evaluate(progress)} from progress {progress}");
            */

            book.Book.transform.position = pos;
            yield return null;
        }

        book.Book.transform.position = target;
        book.Book.GetComponentInChildren<Animator>()?.SetTrigger("StopFlying");
        if(!toInitial) book.OnMovedToPosition();

        Debug.Log("[ShopManager] book moved into position");
        _currentlyMovingBooks--;
        yield break; 
    }
}
