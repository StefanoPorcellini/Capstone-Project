import { iAuthData } from './../../models/auth-data';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BehaviorSubject, map, Observable, retry, tap } from 'rxjs';
import { iUser } from '../../models/user';
import { Router } from '@angular/router';
import { iAuthResponse } from '../../models/auth-response';

@Injectable({
  providedIn: 'root'
})
export class AuthService {


  jwtHelper:JwtHelperService = new JwtHelperService()

  authSubject = new BehaviorSubject<null | iUser>(null)

  user$=this.authSubject.asObservable()
  syncIsLoggedIn:boolean = false

  isLoggedIn$ = this.user$.pipe(
    map(user => !!user),
    tap(user => this.syncIsLoggedIn = user))

  constructor(private http: HttpClient, private router:Router) { this.restoreUser() }

  private loginUrl = "https://localhost:7147/api/User/login";
  private createUrl = "https://localhost:7147/api/User/create";

  create(newUser:Partial<iUser>):Observable<iAuthResponse>{
    return this.http.post<iAuthResponse>(this.createUrl, newUser)
  }

  login(authData:iAuthData):Observable<iAuthResponse>{
    return this.http.post<iAuthResponse>(this.loginUrl, authData)
    .pipe(tap(data=>{
      this.authSubject.next(data.user)
      localStorage.setItem('accessData', JSON.stringify(data))
      this.autoLogout()
    }))
  }

  logout():void{
    this.authSubject.next(null)
    localStorage.removeItem('accessData')
    this.router.navigate( ['/auth/login'])
  }

  autoLogout():void{
    const accessData = this.getAccessData()
    if(!accessData) return
    const expDate = this.jwtHelper.getTokenExpirationDate(accessData.accessToken) as Date
    const expMs = expDate.getTime() - new Date().getTime()
    setTimeout(this.logout, expMs)
  }

  getAccessData():iAuthResponse|null{

    const accessDataJson = localStorage.getItem('accessData')
    if(!accessDataJson) return null
    const accessData:iAuthResponse = JSON.parse(accessDataJson)

    return accessData;
  }

  restoreUser():void{
    const accessData = this.getAccessData()
    if(!accessData) return
    if(this.jwtHelper.isTokenExpired(accessData.accessToken)) return
    this.authSubject.next(accessData.user)
    this.autoLogout()
  }

}
