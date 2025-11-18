import { Injectable } from '@angular/core';
import { Observable, Subject, of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CachingService {

  cachingDictionary: any = {};

  constructor() { }

  // Format for key: service.endpoint. Must be unique
  // Default timeout is 16 minutes. Refreshing resets all of this
  getCachedResult<T>(key: string, call: Observable<T>, timeout: number = 1000000, refresh: boolean = false): Observable<T> {
    let cachedEntry: CacheEntry<T> = this.cachingDictionary[key];
    if (cachedEntry && !refresh) {
      if (cachedEntry.cachedResponse) {
        return of(cachedEntry.cachedResponse);
      }
      if (cachedEntry.isPending) {
        return cachedEntry.responseIou;
      }
    }
    else {
      cachedEntry = {
        isPending: false,
        responseIou: new Subject<T>,
        cachedResponse: undefined
      };
      this.cachingDictionary[key] = cachedEntry;

    }
    cachedEntry.isPending = true;
    return call.pipe(
      tap(x => {
        cachedEntry.cachedResponse = x;
        cachedEntry.responseIou.next(x);
        cachedEntry.isPending = false;
        setTimeout(() => {
          cachedEntry.cachedResponse = undefined;
        }, timeout);
      })
    );
  }

  clearCachedResult<T>(key: string) {
    let cache = {
      isPending: false,
      responseIou: new Subject<T>,
      cachedResponse: undefined
    }
    this.cachingDictionary[key] = cache;
    
  }

}

// Typing causes cached response to comp
interface CacheEntry<T> {
  isPending: boolean;
  responseIou: Subject<T>,
  cachedResponse?: T
}
