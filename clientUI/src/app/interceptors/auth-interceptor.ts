import { HttpEvent, HttpHandler, HttpHandlerFn, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { catchError, Observable, of, switchMap } from "rxjs";
import { AuthService } from "../auth/auth.service";
import { environment } from "../environment";



export const AuthenticationInterceptor = (req: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> => {
    const authService = inject(AuthService);
    const isToServer = req.url.startsWith(environment.apiUrl);
    if (!isToServer) {
        return next(req);
    }

    if (authService.isLoggedIn()) {
        const cloned = req.clone({
            setHeaders: {
                Authorization: `Bearer ${authService.accessToken}`
            }
        });
        return next(cloned);
    }

    return next(req);
}
