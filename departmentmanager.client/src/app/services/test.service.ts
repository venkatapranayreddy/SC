import { inject, Injectable } from "@angular/core";
import { BaseService } from "./base.service";
import { HttpParams } from "@angular/common/http";
import { CachingService } from "./caching.service";

@Injectable({ providedIn: 'root' })
export class TestService extends BaseService {
  private cachingService = inject(CachingService);
  constructor() { super('/api/Test/'); }

  get() {
    return this.customGetRequest(null, 'Test', new HttpParams());
  }
  cachedGet() {
    return this.cachingService.getCachedResult<Array<any>>('test.get2',
      this.customGetRequest(null, 'Test', new HttpParams())
    );
  }
}
