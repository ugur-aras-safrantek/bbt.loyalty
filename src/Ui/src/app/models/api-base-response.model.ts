interface IApiBaseResponseModel{
  data: any;
  errorMessage: string;
  hasError: boolean;
}

export class ApiBaseResponseModel implements  IApiBaseResponseModel{
  data: any;
  errorMessage: string;
  hasError: boolean;
}

interface IHttpErrorResponseModel{
  url: string;
  message: string;
  status: string;
  statusText: string;
}

export class HttpErrorResponseModel implements  IHttpErrorResponseModel{
  url: string;
  message: string;
  status: string;
  statusText: string;
}
