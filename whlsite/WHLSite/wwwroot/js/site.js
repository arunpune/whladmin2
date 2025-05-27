// Helper functions for UI validations

function fnIsValidUsername(input) {
  if (input === undefined || input === null) input = '';
  console.debug('Validating username - ' + input);
  //var regex = /^[A-Za-z]{1}[A-Za-z0-9]{7,31}$/;
  var regex = /^^(?!.*(.).*\1{2})([A-Za-z]{1}[A-Za-z0-9]{7,31})$/;
  var isValid = regex.test(input);
  console.debug('Username is ' + (isValid ? 'valid' : 'invalid'));
  return isValid;
}

function fnIsValidEmailAddress(input) {
  if (input === undefined || input === null) input = '';
  console.debug('Validating email address - ' + input);
  var regex = /^[A-Za-z0-9._-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$/;
  var isValid = regex.test(input);
  console.debug('Email address is ' + (isValid ? 'valid' : 'invalid'));
  return isValid;
}

function fnIsValidPassword(input) {
  if (input === undefined || input === null) input = '';
  console.debug('Validating password - ' + input);
  var regex = /^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*\W)(?!.* ).{14,}$/;
  var isValid = regex.test(input);
  console.debug('Password is ' + (isValid ? 'valid' : 'invalid'));
  return isValid;
}

function fnIsValidPhoneNumber(input) {
  if (input === undefined || input === null) input = '';
  console.debug('Validating phone number - ' + input);
  var regex = /^[\d]{10}$/;
  var isValid = regex.test(input);
  console.debug('Phone number is ' + (isValid ? 'valid' : 'invalid'));
  return isValid;
}

function fnIsValidName(input) {
  if (input === undefined || input === null) input = '';
  console.debug('Validating name - ' + input);
  var regex = /^([A-Za-z]+([\'\-]?[A-Za-z]+)?)([\ \-]?)(([A-Za-z]+([\'\-]?[A-Za-z]+)?)?)$/;
  var isValid = regex.test(input);
  console.debug('Name is ' + (isValid ? 'valid' : 'invalid'));
  return isValid;
}

function fnIsValidNameWithSpecialCharacters(input) {
  if (input === undefined || input === null) input = '';
  console.debug('Validating name with special characters - ' + input);
  var regex = /^([A-Za-z\u00C0-\u017F]+([\'\-]?[A-Za-z\u00C0-\u017F]+)?)([\ \-]?)(([A-Za-z\u00C0-\u017F]+([\'\-]?[A-Za-z\u00C0-\u017F]+)?)?)$/;
  var isValid = regex.test(input);
  console.debug('Name is ' + (isValid ? 'valid' : 'invalid'));
  return isValid;
}

function fnIsValidSuffix(input) {
  if (input === undefined || input === null) input = '';
  console.debug('Validating suffix - ' + input);
  var regex = /^([A-Za-z]+(\.)?)$/;
  var isValid = regex.test(input);
  console.debug('Suffix is ' + (isValid ? 'valid' : 'invalid'));
  return isValid;
}

function fnIsDefined(input) {
  return input !== undefined && input !== null;
}

function fnCapitalize(input) {
  if (input === undefined || input === null) input = '';
  console.debug('Capitalizing input - ' + input);
  let output = input.toLowerCase().replace(/^(.)|\s(.)/g, function($1) {
    return $1.toUpperCase();
  });
  console.debug('Capitalized is ' + output);
  return output;
}

function fnFormatCurrency(input) {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
    maximumFractionDigits: 0
  }).format(input);
}

// For toast messages
var toastElem = document.getElementById('whlToastMain');
if (toastElem !== undefined && toastElem !== null) {
  toastElem.addEventListener('hidden.bs.toast', () => {
    $('.whl-toast-main').removeClass('bg-success').removeClass('text-white');
    $('.whl-toast-main').removeClass('bg-light').removeClass('text-dark');
    $('.whl-toast-main').removeClass('bg-danger').removeClass('text-white');
    $('.whl-toast-header').html('Notification');
    $('.whl-toast-message').html('');
  });
}

function fnShowToastForPage(status, message) {

  var whlToast = bootstrap.Toast.getOrCreateInstance(toastElem);
  whlToast.show();
  if (status === 'SUCCESS') {
    $('.whl-toast-main').addClass('bg-success').addClass('text-white');
    $('.whl-toast-header').html('Success');
  } else if (status === 'FAILURE') {
    $('.whl-toast-main').addClass('bg-danger').addClass('text-white');
    $('.whl-toast-header').html('Failed');
  } else {
    $('.whl-toast-main').addClass('bg-light').addClass('text-dark');
    $('.whl-toast-header').html('Notification');
  }
  $('.whl-toast-message').html(message);

}

function fnShowToast(status, message, navigateToUrl) {

  var whlToast = bootstrap.Toast.getOrCreateInstance(toastElem);
  whlToast.show();
  if (status === 'SUCCESS') {
    $('.whl-toast-main').addClass('bg-success').addClass('text-white');
    $('.whl-toast-header').html('Success');
  } else if (status === 'FAILURE') {
    $('.whl-toast-main').addClass('bg-danger').addClass('text-white');
    $('.whl-toast-header').html('Failed');
  } else {
    $('.whl-toast-main').addClass('bg-light').addClass('text-dark');
    $('.whl-toast-header').html('Notification');
  }
  $('.whl-toast-message').html(message);

  var navigateTimeout = setTimeout(function () {
    clearTimeout(navigateTimeout);
    if (navigateToUrl !== undefined && navigateToUrl !== null && navigateToUrl.trim().length > 0) {
      window.location.href = navigateToUrl;
    } else {
      window.location.reload();
    }
  }, 2000);

}