let addressType = '';
let addressCandidates = [];
let selectedAddress = {};

function fnSanitize(v) {
  if (v === undefined || v === null) v = '';
  return v.trim();
}

function fnSelectAddress(i) {
  if (i === -1) {
    console.debug('Using entered address');
    $('.whl-formfield-' + addressType + 'county').val('OTHER');
    $('.whl-formfield-' + addressType + 'county').addClass('is-valid');
  } else {
    selectedAddress = addressCandidates[i];
    console.debug(selectedAddress);
    $('.whl-formfield-' + addressType + 'streetline1').val(fnCapitalize(selectedAddress.streetLine1));
    $('.whl-formfield-' + addressType + 'streetline1').removeClass('is-invalid').addClass('is-valid');
    if (fnIsDefined(selectedAddress.streetLine2) && selectedAddress.streetLine2.trim().length > 0) {
      $('.whl-formfield-' + addressType + 'streetline2').val(fnCapitalize(selectedAddress.streetLine2));
      $('.whl-formfield-' + addressType + 'streetline2').removeClass('is-invalid').addClass('is-valid');
    }
    if (fnIsDefined(selectedAddress.streetLine3) && selectedAddress.streetLine3.trim().length > 0) {
      $('.whl-formfield-' + addressType + 'streetline3').val(fnCapitalize(selectedAddress.streetLine3));
      $('.whl-formfield-' + addressType + 'streetline3').removeClass('is-invalid').addClass('is-valid');
    }
    $('.whl-formfield-' + addressType + 'city').val(fnCapitalize(selectedAddress.city));
    $('.whl-formfield-' + addressType + 'city').removeClass('is-invalid').addClass('is-valid');
    $('.whl-formfield-' + addressType + 'statecd').val(selectedAddress.state);
    $('.whl-formfield-' + addressType + 'statecd').removeClass('is-invalid').addClass('is-valid');
    $('.whl-formfield-' + addressType + 'zipcode').val(selectedAddress.zipCode);
    $('.whl-formfield-' + addressType + 'zipcode').removeClass('is-invalid').addClass('is-valid');
    $('.whl-formfield-' + addressType + 'county').val(selectedAddress.county.toUpperCase());
    $('.whl-formfield-' + addressType + 'county').removeClass('is-invalid').addClass('is-valid');
  }
  $('#addressCandidatesModal').modal('hide');
  $('.whl-formfieldrow-available-addresses').hide();
  $('.whl-formfields-' + addressType + 'addressvalidated').hide();
  $('.whl-formfield-' + addressType + 'addressvalidated').val('1');
  $('.whl-address-candidates').html('<div class="row"><div class="col-xs-12">None</div></div>');
  $('.whl-formfield-required-validation').removeClass('is-valid');
  $('.whl-formfield-required-validation').removeClass('is-invalid');
  $('.whl-formfield-required-save').removeClass('is-valid');
  $('.whl-formfield-required-save').removeClass('is-invalid');
  $('.whl-formfield-required-save').addClass('is-valid');
  fnToggleSaveAddressInfo();
  return false;
}

$(function () {

  $('.whl-action-save-addressinfo').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-action-cancel-addressinfo').hide();
    $('.whl-action-save-addressinfo').attr('disabled', 'disabled');
    $('.whl-formfield-physicalcounty').removeAttr('disabled');
    $('.whl-formfield-mailingcounty').removeAttr('disabled');
    $('.whl-action-save-addressinfo').html('Saving...');
    $('.whl-form-edit-addressinfo').submit();
  });

  $('.whl-action-validate-address').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();

    addressType = $(e.currentTarget).data('addresstype');

    let psl1 = fnSanitize($('.whl-formfield-' + addressType + 'streetline1').val());
    let psl2 = fnSanitize($('.whl-formfield-' + addressType + 'streetline2').val());
    let psl3 = fnSanitize($('.whl-formfield-' + addressType + 'streetline3').val());
    let psc = fnSanitize($('.whl-formfield-' + addressType + 'city').val());
    let pss = fnSanitize($('.whl-formfield-' + addressType + 'statecd').val());
    let psz = fnSanitize($('.whl-formfield-' + addressType + 'zipcode').val());
    let psct = fnSanitize($('.whl-formfield-' + addressType + 'county').val());

    let sa = psl1;
    sa += psl2.length > 0 ? ((sa.length > 0 ? ' ' : '') + psl2) : '';
    sa += psl3.length > 0 ? ((sa.length > 0 ? ' ' : '') + psl3) : '';

    let csz = psc;
    csz += pss.length > 0 ? ((csz.length > 0 ? ' ' : '') + pss) : '';
    csz += psz.length > 0 ? ((csz.length > 0 ? ' ' : '') + psz) : '';

    let streetAddress = sa;
    streetAddress += csz.length > 0 ? ((streetAddress.length > 0 ? ' ' : '') + csz) : '';

    $('.whl-action-validate-' + addressType + 'address').attr('disabled', 'disabled');
    $('.whl-action-validate-' + addressType + 'address').html('Validating...');
    $('#lblEnteredAddressStreet').html(sa);
    $('#lblEnteredAddressCity').html(psc);
    $('#lblEnteredAddressStateCd').html(pss);
    $('#lblEnteredAddressZipCode').html(psz);
    $('#lblEnteredAddressCounty').html(psct);
    $('#addressCandidatesModal').modal('show');

    let addressParts = 'Address=' + encodeURIComponent(streetAddress);

    let addressCandidatesUrl = arcGisAddressCandidatesApiUrl + '?' + addressParts + '&outFields=*&f=pjson&forStorage=false';
    if (arcGisApiToken !== undefined && arcGisApiToken !== null && arcGisApiToken.trim().length > 0) {
      addressCandidatesUrl += '&api_key=' + arcGisApiToken;
    }
    $.get(addressCandidatesUrl)
      .done(function (response) {
        console.debug(response);
        let liHtml = '';
        const candidates = JSON.parse(response).candidates;
        addressCandidates = [];
        selectedAddress = {};
        let addressCtr = 0;
        $(candidates).each(function (i, elem) {
          const singleLine = fnSanitize(elem.address);
          const streetAddress1 = fnSanitize(elem.attributes.StAddr);
          const streetAddress2 = fnSanitize(elem.attributes.SubAddr);
          const streetAddress = streetAddress1 + (streetAddress2.length > 0 ? (', ' + streetAddress2) : '');
          const city = fnSanitize(elem.attributes.City);
          const region = fnSanitize(elem.attributes.Region);
          const regionAbbr = fnSanitize(elem.attributes.RegionAbbr);
          const state = regionAbbr.length == 2 ? regionAbbr : region;
          const zipCode = fnSanitize(elem.attributes.Postal);
          const subRegion = fnSanitize(elem.attributes.Subregion);
          const county = subRegion.length > 0 ? subRegion : 'Other';
          const longitude = fnSanitize('' + elem.location.x);
          const latitude = fnSanitize('' + elem.location.y);
          if (elem.score > 80.0 && county.length > 0) {
            addressCandidates.push({ id: addressCtr, singleLine: singleLine, streetLine1: streetAddress1, streetLine2: streetAddress2, city: city, state: state, zipCode: zipCode, county: county, longitude: longitude, latitude: latitude, x: elem.location.x, y: elem.location.y });
            liHtml += '<div class="row">'
              + '<div class="col-xs-12 col-md-1"><small><a href="javascript:void(0);" class="whl-link fw-bolder whl-address-candidate" title="' + elem.address + '" onclick="return fnSelectAddress(' + addressCtr + ')">SELECT</a></small></div>'
              + '<div class="col-xs-12 col-md-4"><small>' + streetAddress + '</small></div>'
              + '<div class="col-xs-12 col-md-3"><small>' + city + '</small></div>'
              + '<div class="col-xs-12 col-md-1"><small>' + state + '</small></div>'
              + '<div class="col-xs-12 col-md-1"><small>' + zipCode + '</small></div>'
              + '<div class="col-xs-12 col-md-2"><small>' + county + '</small></div>'
              + '</div>';
            addressCtr++;
          }
        });
        if (liHtml.length === 0) liHtml += '<div class="row"><div class="col-xs-12">None</div></div>';
        $('.whl-address-candidates').html(liHtml);
        $('.whl-formfieldrow-available-addresses').show();
      })
      .fail(function (err) {
        console.error(err);
      });
  });

});