import { HttpEvent, HttpHandlerFn, HttpRequest } from "@angular/common/http";
import { inject } from "@angular/core";
import { catchError, Observable, switchMap } from "rxjs";
import { AuthService } from "../auth/auth.service";

export const RefreshTokenInterceptor = (req: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> => {
    req = req.clone({ withCredentials: true });
    const authService = inject(AuthService);
    return next(req).pipe(
        catchError((error) => {
            if (error.status === 401) {
                return authService.renewAuthToken().pipe(
                    switchMap(() => {
                        const cloned = req.clone({
                            setHeaders: {
                                Authorization: `Bearer ${authService.accessToken}`
                            }
                        });
                        return next(cloned);
                    }),
                    catchError(() => {
                        authService.logout();
                        return next(req);
                    })
                );
            }
            return next(req);
        }));
}