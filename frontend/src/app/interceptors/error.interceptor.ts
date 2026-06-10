import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Er is een onbekende fout opgetreden.';

      if (error.status === 0) {
        errorMessage = 'De server is onbereikbaar. Controleer of de backend draait.';
        // Je zou hier eventueel een globale melding (toast/snackbar) kunnen tonen
        console.error('Netwerkfout: De backend lijkt offline te zijn.');
      } else if (error.status === 401) {
        errorMessage = 'Sessie verlopen of niet geautoriseerd.';
      } else if (error.status === 403) {
        errorMessage = 'Je hebt geen toestemming voor deze actie.';
      } else if (error.error && typeof error.error === 'string') {
        errorMessage = error.error;
      } else if (error.message) {
        errorMessage = error.message;
      }

      // We geven de fout door aan de component, maar voegen de verbeterde boodschap toe
      return throwError(() => ({ ...error, friendlyMessage: errorMessage }));
    })
  );
};
