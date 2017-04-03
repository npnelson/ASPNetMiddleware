# ASPNetMiddleware
small middlewares you might find helpful


**RequireHTTPSExceptForLocalhost**
When doing testing on my dev machine, I am running Kestrel on localhost. In production, I am usually reverse proxying through IIS or another web server to kestrel and I let the reverse proxy handle SSL. Consequently, I want to require HTTPS unless I am testing on localhost because I find setting up SSL on localhost cumbersome.

Enter the RequireHTTPSExceptForLocalHost middleware. To use it, simply place it in your Configure method of Starup like so:

``` public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        { 
             //other configure code...
             app.UseRequireHttpsExceptForLocalHostMiddleware();
             //other pipeline stuff here
        }
```
**Important** you will want to use the middleware high up in the pipeline, any request that comes before the requirehttps middleware in the pipeline will **NOT** be redirected to HTTPS

**RequireAuthenticationExceptforLocalHost**

Same usage and concept, but will not require authentication when running localhost. I find this useful when testing some of my webAPIs
