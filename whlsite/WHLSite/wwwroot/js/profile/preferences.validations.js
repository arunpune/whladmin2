$(function () {

  // Disable submission
  $('.whl-action-save-preferences').attr('disabled', 'disabled');
  fnToggleLanguagePreferenceOther();
  fnToggleSavePreferences();

  $('.whl-formfield-languagepreferencecd').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleLanguagePreferenceOther();
    fnToggleSavePreferences();
  });

  $('.whl-formfield-languagepreferenceother').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSavePreferences();
  });

  $('.whl-formfield-listingpreferencecd').on('change', function (e) {
    $(this).removeClass('is-valid');
    $(this).removeClass('is-invalid');
    var v = $(this).val();
    if (v !== undefined && v !== null && v.trim().length > 0) {
      $(this).addClass('is-valid');
    } else {
      $(this).addClass('is-invalid');
    }
    fnToggleSavePreferences();
  });

  function fnToggleLanguagePreferenceOther() {
    $('.whl-formfield-languagepreferenceother').removeClass('is-valid');
    $('.whl-formfield-languagepreferenceother').removeClass('is-invalid');
    var v = $('.whl-formfield-languagepreferencecd').val();
    if (v !== undefined && v !== null && v.trim().length > 0 && v.toUpperCase() === 'OTHER') {
      $('.whl-formfieldcol-languagepreferenceother').show();
      var lpo = $('.whl-formfield-languagepreferenceother').val();
      if (lpo !== undefined && lpo !== null && lpo.trim().length > 0) {
        $('.whl-formfield-languagepreferenceother').addClass('is-valid');
      } else {
        $('.whl-formfield-languagepreferenceother').addClass('is-invalid');
      }
    } else {
      $('.whl-formfieldcol-languagepreferenceother').hide();
      $('.whl-formfield-languagepreferenceother').val('');
    }
  }

  function fnToggleSavePreferences() {
    var lg = $('.whl-formfield-languagepreferencecd').val();
    var lpo = $('.whl-formfield-languagepreferenceother').val();
    var lt = $('.whl-formfield-listingpreferencecd').val();
    var allowSubmission = lg !== undefined && lg !== null && lg.trim().length > 0
      && lt !== undefined && lt !== null && lt.trim().length >= 0;
    if (allowSubmission && lg.trim().toUpperCase() === 'OTHER') {
      allowSubmission = lpo !== undefined && lpo !== null && lpo.trim().length > 0;
    }
    if (allowSubmission) {
      $('.whl-action-save-preferences').removeAttr('disabled');
    }
    else {
      $('.whl-action-save-preferences').attr('disabled', 'disabled');
    }
  }

});