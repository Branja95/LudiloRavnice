import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
    constructor() {}
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        
    let jwt = localStorage.jwt;

    if (jwt) 
    {
        request = request.clone(
            {
                setHeaders: 
                { 
                    Authorization: `Bearer ${jwt}`
                }
            });
    }

    return next.handle(request);
  }
}