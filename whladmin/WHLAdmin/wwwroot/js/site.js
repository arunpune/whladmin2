// Helper functions for UI validations

function fnIsValidUsername(input) {
  if (input === undefined || input === null) input = '';
  console.debug('Validating username - ' + input);
  var regex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
  var isValid = regex.test(input);
  console.debug('Username is ' + (isValid ? 'valid' : 'invalid'));
  return isValid;
}

function fnIsValidPassword(input) {
  if (input === undefined || input === null) input = '';
  console.debug('Validating password - ' + input);
  var regex = /^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*\W)(?!.* ).{14,}$/;
  var isValid = regex.test(input);
  console.debug('Username is ' + (isValid ? 'valid' : 'invalid'));
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

function fnIsValidRate(input) {
  if (input === undefined || input === null) input = '';
  console.debug('Validating rate - ' + input);
  var regex = /^([1-9]?[0-9]?)((\.)?\d{1,5})?$/;
  var isValid = regex.test(input) && parseFloat(input) > 0 && parseFloat(input) <= 100;
  console.debug('Rate is ' + (isValid ? 'valid' : 'invalid'));
  return isValid;
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

function fnIsValidFileSize(elemId, maxSize) {
  const files = document.getElementById(elemId).files;
  if (files.length > 0) {
    const file = files[0];
    const fileSizeMb = ((file.size / 1024) / 1024);
    if (fileSizeMb > 0 && fileSizeMb <= maxSize) {
      return true;
    }
    alert('Empty file, or file is larger than 1MB!');
    document.getElementById(elemId).value = null;
  }
  return false;
}

function fnGetBase64(elemId, outputElemId, fileNameElemId) {
  const files = document.getElementById(elemId).files;
  if (files.length > 0) {
    const file = files[0];
    if (elemId === 'imageFile') {
      $('#MimeType').val(file.type);
    } else if (elemId === 'documentFile') {
      $('#DocumentMimeType').val(file.type);
    }
    if (fileNameElemId !== undefined && fileNameElemId !== null && fileNameElemId.trim().length > 0) {
      $('#' + fileNameElemId).val(file.name);
    }
    var fileReader = new FileReader();
    fileReader.onload = function (fileLoadedEvent) {
      var srcData = fileLoadedEvent.target.result;
      document.getElementById(outputElemId).value = srcData;
    }
    fileReader.readAsDataURL(file);
  }
}