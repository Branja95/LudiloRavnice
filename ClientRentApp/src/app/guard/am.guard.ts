import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';

@Injectable()
export class AMGuard implements CanActivate {

  constructor() {}

  canActivate() {
    return localStorage.role == 'Administrator' || localStorage.role == 'Manager';
  }
}