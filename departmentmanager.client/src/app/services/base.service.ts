import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Inject, Injectable } from '@angular/core';
import { BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { catchError, Observable, throwError } from 'rxjs';
import { AuthService } from './auth.service';
import { ModalMessage } from '../models/modal-message';
import { ModalInformComponent } from '../core/modal-inform/modal-inform.component';
import { BASE_URL } from '../app.config';
import { StkModalService } from './modal.service';

export abstract class BaseService {

  private baseUrl: string = inject(BASE_URL);
  modalService: StkModalService = inject(StkModalService);
  httpClient: HttpClient = inject(HttpClient);
  authService: AuthService = inject(AuthService);
  constructor(controller: string) { this.baseUrl += controller; }

  protected getRequest(id: number | string | null, parameters: HttpParams | undefined, supressError: boolean = false): Observable<any> {
    return this.getChildRequest(id, null, null, parameters, supressError);
  }

  protected customGetRequest(id: number | string | null, action: string, parameters: HttpParams, supressError: boolean = false): Observable<any> {
    return this.customGetChildRequest(id, null, null, action, parameters, supressError);
  }

  protected customGetFileRequest(id: number | string | null, action: string, params: HttpParams, supressError: boolean = false): Observable<any> {
    var headers: HttpHeaders = this.buildBaseFileRequestHeaders();
    return this.httpClient
      .get<any>(this.baseUrl + this.addDataToUrl(id) + this.addDataToUrl(action), { headers: headers, params: params, responseType: 'blob' as 'json' })
      .pipe(
        catchError(err => {
          return this.handleError(err, supressError);
        })
      );
  }

  protected getChildRequest(id: number | string | null, childEntity: string | null, childId: number | string | null = null, params: HttpParams | undefined, supressError: boolean = false): Observable<any> {
    var headers: HttpHeaders = this.buildBaseJsonRequestHeaders();
    return this.httpClient
      .get<any>(this.baseUrl + this.addDataToUrl(id) + this.addDataToUrl(childEntity) + this.addDataToUrl(childId), { headers: headers, params: params })
      .pipe(
        catchError(err => {
          return this.handleError(err, supressError);
        })
      );
  }

  protected customGetChildRequest(id: number | string | null, childEntity: string | null, childId: number | string | null = null, action: string, params: HttpParams | undefined, supressError: boolean = false): Observable<any> {
    var headers: HttpHeaders = this.buildBaseJsonRequestHeaders();
    console.log(this.baseUrl + this.addDataToUrl(id) + this.addDataToUrl(childEntity) + this.addDataToUrl(childId) + action);
    return this.httpClient
      .get<any>(this.baseUrl + this.addDataToUrl(id) + this.addDataToUrl(childEntity) + this.addDataToUrl(childId) + action, { headers: headers, params: params })
      .pipe(
        catchError(err => {
          return this.handleError(err, supressError);
        })
      );
  }

  protected postRequest(action: string | null, jsonData: string, supressError: boolean = false): Observable<any> {
    return this.postChildRequest(null, null, action, jsonData);
  }

  protected postChildRequest(id: number | null, childEntity: string | null, action: string | null, jsonData: string, supressError: boolean = false): Observable<any> {
    var headers: HttpHeaders = this.buildBaseJsonRequestHeaders();
    return this.httpClient
      .post<any>(this.baseUrl + this.addDataToUrl(id) + this.addDataToUrl(childEntity) + (!action ? '' : action), jsonData, { headers: headers })
      .pipe(
        catchError(err => {
          return this.handleError(err, supressError);
        })
      );
  }

  protected postWithFileRequest(action: string | null, obj: any, files: Array<File>, supressError: boolean = false) {
    let formData = new FormData();
    let keys = Object.keys(obj);
    for (let k of keys) {
      if (obj[k] === null || obj[k] === undefined) {
        continue;
      }

      this.createFormData(formData, obj[k], k);

    }
    if (files) {
      for (var f of files) {
        formData.append('files', f);
      }
    }

    return this.httpClient.post<any>(this.baseUrl + action || '', formData, { headers: this.buildBaseFormWithFileRequestHeaders() }).pipe(
      catchError(err => {
        return this.handleError(err, supressError);
      })
    );
  }

  protected putWithFileRequest(action: string | null, obj: any, files: Array<File>, supressError: boolean = false) {
    let formData = new FormData();
    let keys = Object.keys(obj);
    for (let k of keys) {
      if (obj[k] === null || obj[k] === undefined) {
        continue;
      }
      this.createFormData(formData, obj[k], k);

    }
    if (files) {
      for (var f of files) {
        formData.append('files', f);
      }
    }

    return this.httpClient.put<any>(this.baseUrl + action || '', formData, { headers: this.buildBaseFormWithFileRequestHeaders() }).pipe(
      catchError(err => {
        return this.handleError(err, supressError);
      })
    );
  }

  private createFormData(formData: FormData, data: any, key: any): void {
    if ((typeof data === 'object' && data !== null) || Array.isArray(data)) {
      for (let i in data) {
        if ((typeof data[i] === 'object' && data[i] !== null) || Array.isArray(data[i])) {
          this.createFormData(formData, data[i], key + '[' + i + ']');
        } else {
          formData.append(key + '[' + i + ']', data[i]);
        }
      }
    } else {
      formData.append(key, data);
    }
  }

  protected putRequest(id: number | string, jsonData: string, supressError: boolean = false): Observable<any> {
    return this.putChildRequest(id, null, null, jsonData);
  }

  protected customPutRequest(id: number | string | null, action: string, jsonData: string, supressError: boolean = false): Observable<any> {
    return this.customPutChildRequest(id, null, null, action, jsonData);
  }

  protected putChildRequest(id: number | string, childEntity: string | null, childId: number | null, jsonData: string, supressError: boolean = false): Observable<any> {
    var headers: HttpHeaders = this.buildBaseJsonRequestHeaders();
    return this.httpClient
      .put(this.baseUrl + this.addDataToUrl(id) + this.addDataToUrl(childEntity) + this.addDataToUrl(childId), jsonData, { headers: headers })
      .pipe(
        catchError(err => {
          return this.handleError(err, supressError);
        })
      );
  }

  protected customPutChildRequest(id: number | string | null, childEntity: string | null, childId: number | null, action: string, jsonData: string, supressError: boolean = false): Observable<any> {
    var headers: HttpHeaders = this.buildBaseJsonRequestHeaders();
    return this.httpClient
      .put(this.baseUrl + this.addDataToUrl(id) + this.addDataToUrl(childEntity) + this.addDataToUrl(childId) + action, jsonData, { headers: headers })
      .pipe(
        catchError(err => {
          return this.handleError(err, supressError);
        })
      );
  }

  protected deleteRequest(id: string, supressError: boolean = false): Observable<any> {
    return this.deleteChildRequest(id, null, null);
  }

  protected deleteChildRequestAction(action: string, childEntity: string | number | null, childId: number | null, supressError: boolean = false): Observable<any> {
    var headers: HttpHeaders = this.buildBaseJsonRequestHeaders();
    return this.httpClient
      .delete<any>(this.baseUrl + this.addDataToUrl(action) + this.addDataToUrl(childEntity) + this.addDataToUrl(childId), { headers: headers })
      .pipe(
        catchError(err => {
          return this.handleError(err, supressError);
        })
      );
  }

  protected customDeleteRequest(id: number | string | null, action: string, supressError: boolean = false): Observable<any> {
    return this.customDeleteChildRequest(id, null, null, action);
  }

  protected deleteChildRequest(id: string, childEntity: string | null, childId: number | null, supressError: boolean = false): Observable<any> {
    var headers: HttpHeaders = this.buildBaseJsonRequestHeaders();
    return this.httpClient
      .delete<any>(this.baseUrl + this.addDataToUrl(id) + this.addDataToUrl(childEntity) + this.addDataToUrl(childId), { headers: headers })
      .pipe(
        catchError(err => {
          return this.handleError(err, supressError);
        })
      );
  }

  protected customDeleteChildRequest(id: number | string | null, childEntity: string | null, childId: number | null, action: string, supressError: boolean = false): Observable<any> {
    var headers: HttpHeaders = this.buildBaseJsonRequestHeaders();
    return this.httpClient
      .delete<any>(this.baseUrl + this.addDataToUrl(id) + this.addDataToUrl(childEntity) + this.addDataToUrl(childId) + action, { headers: headers })
      .pipe(
        catchError(err => {
          return this.handleError(err, supressError);
        })
      );
  }

  protected customReportFileRequest(id: number | string | null, action: string, jsonData: string, supressError: boolean = false): Observable<any> {
    var headers: HttpHeaders = this.postBuildBaseFileRequestHeaders();
    return this.httpClient
      .post<any>(this.baseUrl + this.addDataToUrl(id) + this.addDataToUrl(action), jsonData, { headers: headers, responseType: 'blob' as 'json' })
      .pipe(
        catchError(err => {
          return this.handleError(err, supressError);
        })
      );
  }

  protected buildBaseJsonRequestHeaders() {
    let httpHeaders = new HttpHeaders()
      .set('Content-Type', 'application/json')
      .set('Accept', 'application/json')
      // Readd when the auth service is ready
      //.set('Authorization', 'Bearer ' + this.authService.auth_token);
      //.set('UserToken', 'Bearer ' + this.authService.auth_token)
      //.set('UniqueId', this.authService.userID);
    return httpHeaders;
  }

  protected buildBaseFileRequestHeaders() {
    let httpHeaders = new HttpHeaders()
      .set('Content-Type', '*/*')
      .set('Accept', '*/*')
      // Readd when the auth service is ready
      //.set('Authorization', 'Bearer ' + this.authService.auth_token);
      //.set('UserToken', 'Bearer ' + this.authService.auth_token)
      //.set('UniqueId', this.authService.userID);
    return httpHeaders;
  }

  protected postBuildBaseFileRequestHeaders() {
    let httpHeaders = new HttpHeaders()
      .set('Content-Type', 'application/json')
      .set('Accept', '*/*')
      // Readd when the auth service is ready
      //.set('Authorization', 'Bearer ' + this.authService.auth_token);
      //.set('UserToken', 'Bearer ' + this.authService.auth_token)
      //.set('UniqueId', this.authService.userID);
    return httpHeaders;
  }

  protected buildBaseFormWithFileRequestHeaders() {
    let httpHeaders = new HttpHeaders()
      .set('Accept', 'application/json, text/plain, */*')
      // Readd when the auth service is ready
      //.set('Authorization', 'Bearer ' + this.authService.auth_token);
      //.set('UserToken', 'Bearer ' + this.authService.auth_token)
      //.set('UniqueId', this.authService.userID);
    return httpHeaders;
  }

  private handleError(error: any, supressError: boolean = false): Observable<never> {
    let errorMessage: ModalMessage = {
      title: 'An error has occured',
      message: ''
    };
    let title = 'An error has occured';
    let message = '';
    if (error.error && error.error.title && error.error.message) {
      errorMessage.title = error.error.title
      errorMessage.message = error.error.message;
    }
    // If it was just an internal error, and we don't pass a custom message
    else if (error.status == 500) {
      errorMessage.message = 'There was internal server error. Please try again later';
    }
    // Unauthenticated error
    else if (error.status == 403 || error.status == 401) {
      errorMessage.title = 'Permission Denied';
      errorMessage.message = 'You do not have permission to access this page. Please contact your system administrator to gain access.'
    }
    else {
      errorMessage.message = 'Something went wrong. Please try again later';
    }

    if (!supressError) {
      this.modalService.showInformModal(errorMessage.message, errorMessage.title);
    }
    return throwError(error);
  }

  private addDataToUrl(data: any, addEndingSlash: boolean = true): string {
    return data == null ? '' : (data + (addEndingSlash ? '/' : ''));
  }

  protected downloadFile(childEntity: string, params: any): any {
    return this.customGetFileRequest(null, childEntity, params);
  }
}
